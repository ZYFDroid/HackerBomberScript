using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ScriptInterpreter
{
    public class StackStateMachine
    {
        //只读程序区
        public List<Instruction> instructions = new List<Instruction>();
        //运行时内存
        public int ProgramCounter = 0;
        public List<string> runtimeStack = new List<string>();
        public SortedList<string, string> runtimeRegister = new SortedList<string, string>();

        //指令集
        public SortedList<string, CommandHost> InstructionCollection = new SortedList<string, CommandHost>(); 
        //定义指令对应内容的委托
        public delegate void CommandHost(StackStateMachine machine, Instruction instruction);

        public StackStateMachine() {
            initializeInstructions();
            Reset();
        }

        public string RegisterName = "表格";
        public string StackName = "笔记";


        const string COMMAND_PUSH = "插入"; //将内容插入文本栈,内容是 文本
        const string COMMAND_POP = "删除";//删除文本栈最上面的文本
        const string COMMAND_CLEAR = "清空";//清空栈
        const string COMMAND_GET = "获取";//获取寄存器的值并压入栈
        const string COMMAND_SET = "设为";//设置寄存器的值为当前栈顶并弹出

        const string COMMAND_CLONE = "复读";//复制栈顶元素
        const string COMMAND_JOIN = "合并"; //从底到顶合并栈中的元素
        const string COMMAND_PRINT = "输出";//输出栈顶内容(不弹出),或者给定参数
        const string COMMAND_SWAP = "交换";//交换栈顶两个元素

        const string COMMAND_GEN = "生成";//生成命令,生成指定内容并插入栈,包含子命令
        const string COMMAND_CODEC_ENCODE = "编码为";//编码命令,编码栈顶文本,包含子命令
        const string COMMAND_CODEC_DECODE = "解码为";//解码命令,解码栈顶文本,包含子命令


        void initializeInstructions() {
            InstructionCollection.Add(COMMAND_PUSH, new CommandHost(HandlePush));
            InstructionCollection.Add(COMMAND_POP, new CommandHost(HandlePop));
            InstructionCollection.Add(COMMAND_CLEAR, new CommandHost(HandleClear));
            InstructionCollection.Add(COMMAND_GET, new CommandHost(HandleGet));
            InstructionCollection.Add(COMMAND_SET, new CommandHost(HandleSet));
            InstructionCollection.Add(COMMAND_CLONE, new CommandHost(HandleClone));
            InstructionCollection.Add(COMMAND_PRINT, new CommandHost(HandlePrint));
            InstructionCollection.Add(COMMAND_SWAP, new CommandHost(HandleSwap));
            InstructionCollection.Add(COMMAND_GEN, new CommandHost(HandleGen));
            InstructionCollection.Add(COMMAND_JOIN, new CommandHost(HandleJoin));
            InstructionCollection.Add(COMMAND_CODEC_ENCODE, new CommandHost(HandleEncode));
            InstructionCollection.Add(COMMAND_CODEC_DECODE, new CommandHost(HandleDecode));

        }

        public void Compile(string code) {
            string[] lines = code.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string line in lines){
                if (line.Trim().StartsWith("#")) { break; }
                instructions.Add(new Instruction(line.Trim()));
            }
        }

        public void Reset() {
            ProgramCounter = 0;
            runtimeRegister.Clear();
            runtimeStack.Clear();
        }

        public void Step() {
            if (ProgramCounter >= instructions.Count)
            {
                return;
            }
            performInstruction(instructions[ProgramCounter]);
            ProgramCounter++;
            if (ProgramCounter >= instructions.Count) {
                if (null != OnProgramFinish) {
                    OnProgramFinish.Invoke(this, this);
                }
            }
        }

        public void Run() {
            while (ProgramCounter < instructions.Count) {
                Step();
            }
        }
        
        void performInstruction(Instruction instruction) {
            if (InstructionCollection.ContainsKey(instruction.InstructionCode)) {
                CommandHost host = InstructionCollection[instruction.InstructionCode];
                host.Invoke(this, instruction);
                return;
            }
            throw new ArgumentException("没有这样的命令:"+instruction.InstructionCode);
        }

        #region 指令实现

        [Description("这是一个描述")]
        public void HandlePush(StackStateMachine machine, Instruction instruction) {
            if (instruction.Args.Length < 1) { throw new ArgumentException(instruction.InstructionCode + " 命令需要参数"); }
            push(instruction.Args[0].Value);
        }

        public void HandlePop(StackStateMachine machine, Instruction instruction) {
            if (runtimeStack.Count < 1) { throw new InvalidOperationException(StackName+"是空的,删除失败"); }
            pop();
        }

        public void HandleGet(StackStateMachine machine, Instruction instruction) {
            if (instruction.Args.Length < 1) { throw new ArgumentException(instruction.InstructionCode + " 命令需要参数"); }
            push(get(instruction.Args[0].Value));
        }

        public void HandleSet(StackStateMachine machine, Instruction instruction) {
            if (instruction.Args.Length < 1) { throw new ArgumentException(instruction.InstructionCode + " 命令需要1-2个参数"); }
            if (instruction.Args.Length >= 2)
            {
                set(instruction.Args[0].Value, instruction.Args[1].Value);
            }
            else
            {
                set(instruction.Args[0].Value, pop());
            }
        }

        public void HandleClear(StackStateMachine machine, Instruction instruction) {
            runtimeStack.Clear();
        }

        public void HandleJoin(StackStateMachine machine, Instruction instruction) {
            StringBuilder sb = new StringBuilder();
            foreach (string s in runtimeStack)
            {
                sb.Append(s);
            }
            runtimeStack.Clear();
            push(sb.ToString());
        }

        public void HandlePrint(StackStateMachine machine, Instruction instruction) {
            if (instruction.Args.Length > 0)
            {
                if (instruction.Args[0].ArgType == ArgType.ENUM)
                {
                    if (instruction.Args[0].Value == "换行")
                    {
                        print(Environment.NewLine);
                    }
                    else
                    {
                        print(instruction.Args[0].Value);
                    }
                }
                else
                {
                    print(instruction.Args[0].Value);
                }
            }
            else
            {
                if (runtimeStack.Count < 2) { throw new ArgumentException(instruction.InstructionCode + " 需要"+StackName+"有至少一个文本"); }
                print(peek());
            }
        }

        public void HandleSwap(StackStateMachine machine, Instruction instruction) {
            if (runtimeStack.Count < 2) { throw new ArgumentException(instruction.InstructionCode + " 需要" + StackName + "有至少两个文本"); }
            string bottom = pop();
            string top = pop();
            push(bottom);
            push(top);
        }

        public void HandleEncode(StackStateMachine machine, Instruction instruction) {
            if (instruction.Args.Length < 1) { throw new ArgumentException(instruction.InstructionCode + " 命令需要参数"); }
            encode(instruction.Args);
        }

        public void HandleDecode(StackStateMachine machine, Instruction instruction) {
            if (instruction.Args.Length < 1) { throw new ArgumentException(instruction.InstructionCode + " 命令需要参数"); }
            decode(instruction.Args);
        }

        public void HandleGen(StackStateMachine machine, Instruction instruction) {
            if (instruction.Args.Length < 1) { throw new ArgumentException(instruction.InstructionCode + " 命令需要参数"); }
            gen(instruction.Args);
        }

        public void HandleClone(StackStateMachine machine, Instruction instruction) {
            if (runtimeStack.Count < 1) { throw new InvalidOperationException(instruction.InstructionCode + " 命令要求"+StackName+"内至少有一条内容"); }
        }

        #endregion

        #region 内部方法

        const string COMMAND_GEN_QQ = "QQ";
        const string COMMAND_GEN_PASSWORD = "密码";
        const string COMMAND_GEN_NATURALPASSWORD = "自然的密码";
        const string COMMAND_GEN_TIMESTAMP = "时间戳";
        const string COMMAND_GEN_NATRALNAME = "姓名";
        const string COMMAND_GEN_IDCARDNUMBER = "身份证号";
        const string COMMAND_GEN_TELEPHONE = "电话";
        const string COMMAND_GEN_MOBILEPHONE = "手机号";
        const string COMMAND_GEN_EMAIL = "电子邮件";
        const string COMMAND_GEN_BANKCARD = "银行卡号";
        const string COMMAND_GEN_PAYMENTPASSWORD = "支付密码";

        const string COMMAND_GEN_DIRTYWORD = "脏话";
        const string COMMAND_GEN_AWESOMEWORD = "骚话";



        private void gen(CodeArg[] args) {
            switch (args[0].Value) {
                case (COMMAND_GEN_QQ):
                    {
                        push(InstructionUtils.RandomQQNumber());
                    }
                    break;
                case (COMMAND_GEN_PASSWORD):
                    {
                        push(InstructionUtils.RandomPassword());
                    }
                    break;
                case (COMMAND_GEN_TIMESTAMP):
                    {
                        push(InstructionUtils.GetTimestamp().ToString());
                    }
                    break;
                default: {
                        throw new ArgumentException("不能生成这个:"+args[0].Value);
                    }

            }
        }


        const string COMMAND_CODEC_TYPE_BASE64 = "BASE64";//base64编码解码
        const string COMMAND_CODEC_TYPE_URL = "URL";//URL编码解码
        const string COMMAND_CODEC_TYPE_MD5 = "MD5";//MD5编码

        private void encode(CodeArg[] args)
        {
            switch (args[0].Value.ToUpper()) {
                case (COMMAND_CODEC_TYPE_URL):
                    {
                        push(InstructionUtils.UrlEncode(pop()));
                    }
                    break;
                case (COMMAND_CODEC_TYPE_BASE64):
                    {
                        push(InstructionUtils.Base64Encode(pop()));
                    }
                    break;
                case (COMMAND_CODEC_TYPE_MD5):
                    {
                        push(InstructionUtils.MD5Encrypt(pop()).ToLower());
                    }
                    break;

                default:
                    {
                        throw new ArgumentException("不能编码这个:" + args[0].Value);
                    }
            }
        }

        private void decode(CodeArg[] args)
        {
            switch (args[0].Value.ToUpper())
            {
                case (COMMAND_CODEC_TYPE_URL):
                    {
                        push(InstructionUtils.UrlDecode(pop()));
                    }
                    break;
                case (COMMAND_CODEC_TYPE_BASE64):
                    {
                        push(InstructionUtils.Base64Decode(pop()));
                    }
                    break;
                case (COMMAND_CODEC_TYPE_MD5):
                    {
                        throw new ArgumentException("MD5理论上不能解码,只能编码");
                    }
                    break;

                default:
                    {
                        throw new ArgumentException("不能解码这个:" + args[0].Value);
                    }
            }
        }

        private void push(string str) {
            runtimeStack.Add(str);
        }

        private string peek() {
            return runtimeStack[runtimeStack.Count - 1];
        }

        private string pop() {
            string str = peek();
            runtimeStack.RemoveAt(runtimeStack.Count - 1);
            return str;
        }

        private string get(string key) {
            return runtimeRegister[key];
        }

        private void set(string key,string value) {
            if (runtimeRegister.ContainsKey(key))
            {
                runtimeRegister[key] = value;
            }
            else {
                runtimeRegister.Add(key, value);
            }
        }

        private void print(string arg) {
            if (null != OnProgramPrint)
            {
                OnProgramPrint.Invoke(this, arg);
            }
            else {
                Console.Write(arg);
            }
        }

        #endregion

        public event EventHandler<StackStateMachine> OnProgramFinish;
        public event EventHandler<string> OnProgramPrint;
    }
    public static class InstructionUtils
    {
        public static long GetTimestamp()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
            return (long)ts.TotalMilliseconds;
        }
        public static string GenerateRandomSequence(string charpool, int minlen, int maxlen)
        {
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            int len = minlen + (rnd.Next() % (maxlen - minlen + 1));
            for (int i = 0; i < len; i++)
            {
                sb.Append(charpool[rnd.Next() % charpool.Length]);
            }
            return sb.ToString();
        }
        public static string RandomQQNumber()
        {
            return GenerateRandomSequence("1234567890", 7, 10);
        }
        public static string RandomPassword()
        {
            return GenerateRandomSequence("1234567890qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM!@#￥%^&*()_+-=[];,./<>?:\"{}\\`~", 6, 12);
        }
        public static string Base64Encode(string input)
        {
            return Base64Encode(input, Encoding.Default);
        }
        public static string Base64Encode(string input, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(input));
        }
        public static string Base64Decode(string input)
        {
            return Base64Decode(input, Encoding.Default);
        }
        public static string Base64Decode(string input,Encoding encoding) {
            return encoding.GetString(Convert.FromBase64String(input));
        }
        public static string UrlEncode(string raw) { return HttpUtility.UrlEncode(raw); }
        public static string UrlDecode(string raw) { return HttpUtility.UrlDecode(raw); }

        public static string MD5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(strText));
            return BitConverter.ToString(result).Replace("-", "");
        }

    }
    public class Instruction {
        string instructionCode;
        CodeArg[] args;
        public string InstructionCode
        {
            get
            {
                return instructionCode;
            }
        }
        public CodeArg[] Args
        {
            get
            {
                return args;
            }
        }
        public Instruction(string src) {
            string[] words = TextUtil.codeSplit(src);
            instructionCode = words[0];
            List<CodeArg> args = new List<CodeArg>();
            for (int i = 1; i < words.Length; i++) {
                args.Add(new CodeArg(words[i]));
            }
            this.args = args.ToArray();
        }
    }
    public class CodeArg {
        string _Value;
        ArgType _ArgType;
        public string Value
        {
            get
            {
                return _Value;
            }

            set
            {
                _Value = value;
            }
        }
        internal ArgType ArgType
        {
            get
            {
                return _ArgType;
            }

            set
            {
                _ArgType = value;
            }
        }

        public CodeArg(string src) {
            if (src.StartsWith("\""))
            {
                _ArgType = ArgType.TEXT;
                _Value = TextUtil.unescapeText(src); 
            }
            else {
                _ArgType = ArgType.ENUM;
                _Value = src;
            }
        }

        public override string ToString()
        {
            if (_ArgType == ArgType.TEXT) {
                return TextUtil.escapeText(_Value);
            }
            return _Value;
        }
    }
    public enum ArgType {
        ENUM,TEXT
    }
    public static class TextUtil {
        public static string[] codeSplit(String line) {
            List<string> datas = new List<string>();
            StringBuilder current = new StringBuilder();
            int ptr = 0;
            bool inBlock = false;
            bool inText = false;
            do
            {
                char chr = line[ptr];
                if (!inBlock)
                {
                    if (chr == ' ')
                    {
                        ptr++;
                        continue;
                    }
                    else {
                        if (current.Length > 0) {
                            datas.Add(current.ToString());
                            current.Clear();
                        }
                        inBlock = true;
                    }

                }
                if(inBlock){
                    if (current.Length == 0) {
                        inText = chr == '\"';
                        current.Append(chr);
                        ptr++;
                        chr = line[ptr];
                    }
                    if (inText)
                    {
                        current.Append(chr);
                        if (chr == '\"') {
                            inBlock = false;
                        }
                        if (chr == '\\') {
                            ptr++;
                            chr = line[ptr];
                            current.Append(chr);
                        }

                    }
                    else {
                        current.Append(chr);
                        if (chr == ' ') {
                            current.Remove(current.Length - 1, 1);
                            inBlock = false;
                        }
                    }
                }
                ptr++;
            } while (ptr < line.Length);
            if (current.Length > 0)
            {
                datas.Add(current.ToString());
                current.Clear();
            }
            return datas.ToArray();
        }
        public static string unescapeText(string src) {
            if (src.StartsWith("\"") && src.EndsWith("\"") && src.Length>=2) {
                string raw = src.Substring(1, src.Length - 2);
                StringBuilder sb = new StringBuilder();
                int ptr = 0;
                do
                {
                    char chr = raw[ptr];
                    if (chr == '\\')
                    {
                        ptr++;
                        chr = raw[ptr];
                        if (chr == 'n')
                        {
                            sb.Append("\n");
                        }
                        else if (chr == 'r')
                        {
                            sb.Append("\r");
                        }
                        else
                        {
                            sb.Append(chr);
                        }
                    }
                    else {
                        sb.Append(chr);
                    }
                    ptr++;
                } while (ptr < raw.Length);
                return sb.ToString();
            }
            throw new ArgumentException("错误:不正确的字符串:"+src);
        }
        public static string escapeText(string src) {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            foreach (char chr in src.ToCharArray()) {
                if ("\\\"".Contains(chr)) {
                    sb.Append('\\');
                }
                if (chr == '\r') { sb.Append("\\r"); }
                else if (chr == '\n')
                { sb.Append("\\n"); }
                else
                {
                    sb.Append(chr);
                }
            }
            return sb.Append("\"").ToString();
        }
    }

    public class Description : Attribute {
        string description;

        public Description(string description)
        {
            this.description = description;
        }

        public string Value
        {
            get
            {
                return description;
            }
        }
    }
}
