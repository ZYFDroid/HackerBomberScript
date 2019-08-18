using ScriptInterpreter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 专治骗子;

namespace BomberStudio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        StackStateMachine machine = new StackStateMachine();
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.icon;
            foreach (string command in machine.InstructionCollection.Keys) {
                string name = command;
                string description = "没有描述";

                try
                {
                    IEnumerable<CustomAttributeData> attrs = machine.InstructionCollection[name].Method.CustomAttributes;
                    foreach (CustomAttributeData attr in attrs)
                    {
                        if (attr.ConstructorArguments.Count > 0)
                        {
                            description = attr.ConstructorArguments[0].Value.ToString();
                        }
                    }
                }
                catch { }
                listAvailableCommands.Items.Add(new ListViewItem(new string[] {name,description}));
            }

            if (Program.openedFile != "") {
                if (File.Exists(Program.openedFile)) {
                    txtCode.Text = File.ReadAllText(Program.openedFile);
                }
            }
            prefix = " - " + Application.ProductName + " " + Application.ProductVersion;
            updateTitle();
            machine.OnProgramPrint += Machine_OnProgramPrint;
        }

        private void Machine_OnProgramPrint(object sender, string e)
        {
            txtOutput.AppendText(e);
        }

        bool saved = true;
        string prefix = " - Bomber Studio v1.0.0.0";
        void updateTitle() {
            if (Program.openedFile == "")
            {
                Text = (saved?"":"*")+"无标题" + prefix;
            }
            else {
                Text = (saved ? "" : "*")+Program.openedFile + prefix;
            }
        }

        DialogResult saveConfirmation() {
            if (saved) { return DialogResult.Yes; }
            DialogResult result = MessageBox.Show("是否保存文件", "保存文件", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes) {
                if (!performSave()) {
                    return DialogResult.Cancel;
                }
            }
            return result;
        }

        bool performSave() {
            if (Program.openedFile == "")
            {
                if (saveFileDialog1.ShowDialog() != DialogResult.OK) {
                    return false;
                }
                Program.openedFile = saveFileDialog1.FileName;
            }
            File.WriteAllText(Program.openedFile, txtCode.Text);
            saved = true;
            updateTitle();
            return true;
        }

        private void ListAvailableCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListViewItem item = listAvailableCommands.SelectedItems[0];
                txtCommandDesc.Text= item.Text + "\r\n" + item.SubItems[1].Text;
            }
            catch { }
        }

        private void TxtCode_TextChanged(object sender, EventArgs e)
        {
            saved = false;
            updateTitle();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            performSave();
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            performSave();
        }

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            Program.openedFile = saveFileDialog1.FileName;
            File.WriteAllText(Program.openedFile, txtCode.Text);
            saved = true;
            updateTitle();
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!saved) {
                if (saveConfirmation() == DialogResult.Cancel) { return; }
            }
            if (openFileDialog1.ShowDialog() != DialogResult.OK) { return; }

            Program.openedFile = openFileDialog1.FileName;
            txtCode.Text = File.ReadAllText(Program.openedFile);
            saved = true;
            updateTitle();
        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (!saved)
            {
                if (saveConfirmation() == DialogResult.Cancel) { return; }
            }
            Program.openedFile = "";
            txtCode.Text = "";
            saved = true;
            updateTitle();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!saved) {
                if (saveConfirmation() == DialogResult.Cancel) { e.Cancel = true; }
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            try
            {
                resetMachine();
                machine.instructions.Clear();
                machine.Compile(txtCode.Text);
                tabControl1.SelectedIndex = 1;
                loadMachineData();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "编译错误，请检查代码");
            }
        }

        public void loadMachineData() {
            listCommands.Items.Clear();
            if (machine.instructions.Count > 0)
            {
                foreach (Instruction inst in machine.instructions)
                {
                    listCommands.Items.Add(inst.ToString());
                }
                int ptr = machine.ProgramCounter;
                if (ptr >= machine.instructions.Count)
                {
                    ptr = machine.instructions.Count - 1;
                }
                listCommands.Items[ptr].BackColor = Color.YellowGreen;
                listCommands.Items[ptr].EnsureVisible();
            }

            listRegister.Items.Clear();
            foreach (KeyValuePair<string, string> regs in machine.runtimeRegister.ToArray()) {
                listRegister.Items.Add(new ListViewItem(new string[] { regs.Key,TextUtil.escapeText(regs.Value)}));
            }

            listStack.Items.Clear();
            foreach (string stack in machine.runtimeStack)
            {
                listStack.Items.Add(stack);
            }

        }

        public void resetMachine() {
            machine.Reset();
            txtOutput.Text = "";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            resetMachine();
            loadMachineData();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (machine.instructions.Count < 1) { return; }
            try
            {
                machine.Step();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "运行时错误");
            }
            loadMachineData();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (machine.instructions.Count < 1) { return; }
            try
            {
                machine.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "运行时错误");
            }
            loadMachineData();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/ZYFDroid/HackerBomberScript");
        }

        private void 文本转义ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FrmTextEscape().Show();
        }

        private void TestSendWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MessageBox.Show(e.UserState.ToString(), e.ProgressPercentage == 0 ? "提交成功" : "提交失败");
        }

        private void TestSendWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String url = machine.runtimeRegister["提交URL"];
            String method = machine.runtimeRegister["提交方法"];
            String content = machine.runtimeRegister["提交内容"];
            try
            {
                HttpWebRequest req = BomberUtils.MakeHttpRequest(url, content, method);
                string result = BomberUtils.GetHttpResponse(req);
                testSendWorker.ReportProgress(0, result);
            }
            catch (Exception ex) {
                testSendWorker.ReportProgress(1, ex.Message);
            }
        }

        private void TestSendWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnTestSend.Enabled = true;
        }

        private void BtnTestSend_Click(object sender, EventArgs e)
        {
            if (!machine.runtimeRegister.ContainsKey("提交URL")) { MessageBox.Show("需要 提交URL");return; }
            if (!machine.runtimeRegister.ContainsKey("提交方法")) { MessageBox.Show("需要 提交方法"); return; }
            if (!machine.runtimeRegister.ContainsKey("提交内容")) { MessageBox.Show("需要 提交内容"); return; }
            btnTestSend.Enabled = false;
            testSendWorker.RunWorkerAsync(machine);
        }
    }
}
