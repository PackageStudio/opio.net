namespace opio
{
    public class OpioEngine
    {
        public static Dictionary<string, OpioClass> oc = new Dictionary<string, OpioClass>();
        public static List<object> oo = new List<object>();

        public static void StartOpioCommand(string command)
        {
            bool finded = false;
            object value = null;
            foreach (OpioInfo info in OpioInfo.infos)
            {
                info.ApplyInfo(command);
                if (info.isInfo)
                {
                    value = info.re;
                    finded = true;
                    break;
                }
            }
            if (value != null) oo.Add(value);
            if (!finded)
            {
                List<string> commandPoint = command.Split(".").ToList();
                OpioClass nowClass;
                try { nowClass = oc[commandPoint[0]]; }
                catch { nowClass = oc["this"]; }
                commandPoint.RemoveAt(0);


                int i = commandPoint.Count;

                foreach (string key in commandPoint)
                {
                    i--;
                    if (key.Replace(">", "") != key)
                    {
                        string[] cpp = key.Split('>');
                        string cppo = "";
                        int ii = cpp.Length - 1;
                        foreach (string s in cpp)
                        {
                            if (ii != cpp.Length - 1)
                            {
                                if (ii > 0) cppo += s + ">";
                                else cppo += s;
                            }
                            ii--;
                        }
                        if(cppo != null && cppo != "")StartOpioCommands(cppo.Replace(",", ";"));
                        try
                        {
                            ((Action)nowClass.objects[cpp[0]]).Invoke();
                        }
                        catch (Exception e)
                        {
                            (nowClass.objects[cpp[0]] as OpioAction).invoke();
                        }
                    }
                    else if (key.Replace("<", "") != key)
                    {
                        string ikey = key.Replace("<", "");
                        oo.Add(nowClass.objects[ikey]);
                    }
                    else if (key.Replace("=", "") != key)
                    {
                        string[] cpg = key.Split('=');
                        StartOpioCommand(cpg[1]);
                        nowClass.objects[cpg[0]] = oo[oo.Count - 1];
                    }
                    else
                    {
                        OpioClass toc = nowClass.objects[key] as OpioClass;
                        if (i == 0) oo.Add(toc);
                        else if (toc != null) nowClass = toc;
                    }
                }
            }
        }
        public static void StartOpioCommands(string commands)
        {
            string[] coomandsArray = commands.Split(";");
            foreach (string c in coomandsArray)
            {
                StartOpioCommand(c);
            }
        }
        public static object GiveOpioObject(int num = 1)
        {
            object obj = oo[oo.Count - num];
            oo.RemoveAt(oo.Count - num);
            return obj;
        }
    }

    public class OpioAction
    {
        public string OpioCommands;
        public void invoke() { OpioEngine.StartOpioCommands(OpioCommands); }
    }

    public class OpioClass
    {
        public string Name { get; set; }
        public Dictionary<string, object> objects = new Dictionary<string, object>();
    }

    public class OpioInfo
    {
        public static List<OpioInfo> infos = new List<OpioInfo>();
        public string Name { get; set; }
        public bool isInfo;
        public object re;
        public virtual void ApplyInfo(string info)
        {
        }
        public static OpioInfo MakeInfo(string infoName)
        {
            var info = new OpioInfo { Name = infoName };
            infos.Add(info);
            return info;
        }
    }

    public class OpioBasic
    {
        public static void ApplyBasicInfo()
        {
            OpioInfo.infos.Add(new OpioStringInfo { Name = "string" });
            OpioInfo.infos.Add(new OpioIntInfo { Name = "int" });
            OpioInfo.infos.Add(new OpiofloatInfo { Name = "float" });
            OpioInfo.infos.Add(new OpioCharInfo { Name = "char" });
            OpioInfo.infos.Add(new OpioBoolInfo { Name = "bool" });
            OpioInfo.infos.Add(new OpioActionInfo { Name = "action" });
            OpioInfo.infos.Add(new OpioClassObjectInfo { Name = "object" });
            OpioInfo.infos.Add(new OpioClassInfo { Name = "class" });
            OpioInfo.infos.Add(new OpioPracticalInfo { Name = "practical" });

            OpioEngine.oc.Add("String", new OpioStringClass());
            OpioEngine.oc.Add("Int", new OpioIntClass());
            OpioEngine.oc.Add("Float", new OpioFloatClass());
        }


        //infos and rules
        public class OpioStringInfo : OpioInfo
        {
            public static void p(object o) { Console.WriteLine(o.ToString()); }
            public override void ApplyInfo(string info)
            {
                isInfo = info.First() == '\"' && info.Last() == '\"';
                if (isInfo)
                {
                    info = info.Substring(1, info.Length - 2);
                    re = info;
                }
            }
        }
        public class OpioCharInfo : OpioInfo
        {
            public override void ApplyInfo(string info)
            {

                isInfo = info.First() == '\'' && info.Last() == '\'';
                if (isInfo)
                {
                    info = info.Substring(1, info.Length - 2);
                    re = info.ToCharArray()[1];
                }
            }
        }
        public class OpioIntInfo : OpioInfo
        {
            public override void ApplyInfo(string info)
            {
                int i;
                isInfo = int.TryParse(info.First().ToString(), out i);
                if (isInfo)
                {
                }
            }
        }
        public class OpiofloatInfo : OpioInfo
        {
            public override void ApplyInfo(string info)
            {
                float i;
                isInfo = float.TryParse(info, out i);
                re = i;
            }
        }
        public class OpioActionInfo : OpioInfo
        {
            public override void ApplyInfo(string info)
            {
                char[] chars = info.ToCharArray();
                if (chars.Length == 0) return;
                isInfo = chars[0] == '{' && chars[chars.Length - 1] == '}';
                if (isInfo)
                {
                    info = info.Remove(0, 1);
                    info = info.Remove(info.Length - 1, 1);
                    re = new OpioAction { OpioCommands = info };
                }
            }
        }
        public class OpioBoolInfo : OpioInfo
        {
            public override void ApplyInfo(string info)
            {
                isInfo = info == "true" || info == "false";
                if (isInfo) re = (info == "true");
            }
        }
        public class OpioClassObjectInfo : OpioInfo
        {
            public override void ApplyInfo(string info)
            {
                string[] spaces = info.Split(" ");
                isInfo = spaces[0] == "object";
                if (isInfo)
                {
                    OpioEngine.oc["this"].objects.Add(spaces[1], null);
                    if (spaces[2] == "=")
                    {
                        OpioEngine.StartOpioCommand(spaces[3]);
                        OpioEngine.oc["this"].objects[spaces[1]] = OpioEngine.oo[OpioEngine.oo.Count - 1];
                    }
                }
            }
        }
        public class OpioClassInfo : OpioInfo
        {
            public override void ApplyInfo(string info)
            {
                string[] startClass = info.Split("{");
                string[] space = startClass[0].Split(" ");
                isInfo = space[0] == "class" && startClass[1].Last() == '}';
                if (isInfo)
                {
                    OpioClass c = new OpioClass { Name = space[1] };
                    OpioEngine.oc.Add(c.Name, c);
                    OpioEngine.oc["this"] = c;
                    OpioEngine.StartOpioCommands(startClass[1].Remove(startClass[1].Length - 1));
                    re = c;
                }
            }
        }
        public class OpioDepotInfo : OpioInfo
        {
            public class dNull { public static dNull get = new dNull(); } 
            public override void ApplyInfo(string info)
            {
                isInfo = info.First() == '@';
                if (isInfo && info == "@null") re = dNull.get;
            }
        }
        public class OpioPracticalInfo : OpioInfo
        {
            public override void ApplyInfo(string info)
            {
                List<string> spaces = info.Split(" ").ToList();

                //if and else
                if (spaces[0] == "if")
                {
                    isInfo = true;
                    OpioEngine.StartOpioCommands(spaces[1].Replace('~', ';'));
                    bool boolIf = Convert.ToBoolean(OpioEngine.GiveOpioObject());

                    if (boolIf)
                    {
                        OpioEngine.StartOpioCommands(((OpioAction)OpioEngine.oc["this"].objects[spaces[2]]).OpioCommands);
                    }
                }
                //while
                else if (spaces[0] == "while")
                {
                    isInfo = true;

                }
                //loop num
                else if (spaces[0] == "loop")
                {
                    isInfo = true;

                }
                //foreach
                else if (spaces[0] == "foreach")
                {
                    isInfo = true;

                }
                //return
                else if (spaces[0] == "return")
                {
                    isInfo = true;

                }
            }
        }

        //basic classes
        public class OpioStringClass : OpioClass
        {
            public static void Split()
            {
                OpioEngine.oo.Add((OpioEngine.GiveOpioObject(2) as string).Split(OpioEngine.GiveOpioObject(1) as string));
            }
        }
        public class OpioIntClass : OpioClass
        {
            public OpioIntClass()
            {
                objects.Add("Parse", Parse);
                objects.Add("CanParse", CanParse);
            }
            public static void Parse()
            {
                OpioEngine.oo.Add(int.Parse(OpioEngine.GiveOpioObject() as string));
            }

            public static void CanParse()
            {
                int i;
                OpioEngine.oo.Add(int.TryParse(OpioEngine.GiveOpioObject() as string, out i));
            }
        }
        public class OpioFloatClass : OpioClass
        {
            public static void Parse()
            {
                OpioEngine.oo.Add(float.Parse(OpioEngine.GiveOpioObject() as string));
            }

            public static void CanParce()
            {
                float i;
                OpioEngine.oo.Add(float.TryParse(OpioEngine.GiveOpioObject() as string, out i));
            }
        }
    }
}
