﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Windows.Controls;
using Newtonsoft.Json; //NuGet

namespace ProtocolEditor
{
    [DataContract]
    class Var
    {
        [DataMember(Order = 0)]
        public String name;

        [DataMember(Order = 1)]
        public String type;

        [DataMember(Order = 2)]
        public bool isArray;

        [DataMember(Order = 3)]
        public bool isClass;

        [DataMember(Order = 4)]
        public String comment;

		[DataMember(Order = 5)]
		public String arrLenType
		{
			get
			{
				if (_arrLenType == null || _arrLenType == "")
				{
					_arrLenType = "int";
				}
				return _arrLenType;
			}
			set
			{
				_arrLenType = value;
			}
		}

		[IgnoreDataMember]
		private String _arrLenType;

		[IgnoreDataMember]
        public String header
        {
            get
            {
                String str = type;
                if (isArray)
                {
					if (arrLenType == null || arrLenType == "")
					{
						arrLenType = "int";
					}
                    str += "[ " + arrLenType + " ]";
                }

                str += " ";
                str += name;

                if (comment != "")
                {
                    return str + "  --" + comment;
                }
                return str;
            }
        }

        [IgnoreDataMember]
        public object parent;

        [IgnoreDataMember]
        public TreeViewItem item;
    }

    [DataContract]
    class Msg
    {
        [DataMember(Order = 0)]
        public String name;

        [DataMember(Order = 1)]
        public String id;

        [DataMember(Order = 2)]
        public String type;

        [DataMember(Order = 3)]
        public String comment;

        [DataMember(Order = 4)]
        public List<Var> vars = new List<Var>();

        [IgnoreDataMember]
        public int idValue
        {
            get
            {
                int ret = Convert.ToInt32(id.Substring(2), 16);
                return ret;
            }
        }

        [IgnoreDataMember]
        public String header
        {
            get
            {
                if (comment == "")
                {
                    return "[" + id + " " + type + "] " + name;
                }
                return "[" + id + " " + type + "] " + name + "  --" + comment;
            }
        }

        [IgnoreDataMember]
        public object parent;

        [IgnoreDataMember]
        public TreeViewItem item;
    }

    [DataContract]
    class Class
    {
        [DataMember(Order = 0)]
        public String name;

        [DataMember(Order = 1)]
        public String comment;

        [DataMember(Order = 2)]
        public List<Var> vars = new List<Var>();

        [IgnoreDataMember]
        public String header
        {
            get
            {
                if (comment == "")
                {
                    return name;
                }
                return name + "  --" + comment;
            }
        }

        [IgnoreDataMember]
        public TreeViewItem item;
    }

    [DataContract]
    class Group
    {
        [DataMember(Order = 0)]
        public String name;

        [DataMember(Order = 1)]
        public String comment;

        [DataMember(Order = 2)]
        public List<Msg> msgs = new List<Msg>();

		[DataMember(Order = 3)]
		public int codeType
		{
			get
			{
				if (_codeType == null)
				{
					_codeType = 0;
				}
				return (int)_codeType;
			}
			set
			{
				_codeType = value;
			}
		}

		[IgnoreDataMember]
		private int? _codeType;

		[IgnoreDataMember]
        public TreeViewItem item;

        [IgnoreDataMember]
        public String header
        {
            get
            {
                if (comment == "")
                {
                    return name;
                }
                return name + "  --" + comment;
            }
        }
    }

    [DataContract]
    class Config
    {
        [DataMember(Order = 0)]
        public List<Class> classes = new List<Class>();

        [DataMember(Order = 1)]
        public List<Group> groups = new List<Group>();
    }


    class ConfigParser
    {
        public Config loadConfig()
        {
            Config cfg = null;

            String configPath = Path.Combine(Properties.Settings.Default.configPath, "ProtocolEditor.Msg.json");
            if (File.Exists(configPath))
            {
                byte[] jsonBytes = Encoding.Convert(Encoding.GetEncoding("GB2312"), Encoding.UTF8, File.ReadAllBytes(configPath));
                
                using (MemoryStream ms = new MemoryStream(jsonBytes))
                {
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Config));
                    cfg = (Config)jsonSerializer.ReadObject(ms);
                }
            }

            if (cfg == null)
            {
                cfg = new Config();
                saveToFile(cfg);
            }

            return cfg;
        }

        public void saveToFile(Config cfg)
        {
            //DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(cfg.GetType());
            //string jsonStr = "";
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    jsonSerializer.WriteObject(stream, cfg);
            //    jsonStr = Encoding.UTF8.GetString(stream.ToArray());
            //}

            string jsonStr = JsonConvert.SerializeObject(cfg, Formatting.Indented);

            String configPath = Path.Combine(Properties.Settings.Default.configPath, "ProtocolEditor.Msg.json");
            File.WriteAllText(configPath, jsonStr, Encoding.GetEncoding("GB2312"));
        }
    }
}
