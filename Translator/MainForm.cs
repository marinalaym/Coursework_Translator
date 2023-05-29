using System;
using System.IO;
using System.Windows.Forms;

namespace Translator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "C files (*.c)|*.c",
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var fileStream = openFileDialog.OpenFile();
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    var fileContent = reader.ReadToEnd();
                    tbCode.Text = fileContent;
                }
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            tbResult.Clear();
            tbTokens.Clear();
            tb_BZ.Clear();
            dgvTableLexemes.Rows.Clear();
            if (tbCode.Text != string.Empty)
            {
                LexicalAnalysis lexAnalysis = new LexicalAnalysis();
                string[] codeText = tbCode.Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    lexAnalysis.AnalyzeCode(codeText);
                    tbResult.Text += "Лексический анализ завершён. Ошибок не обнаружено.\r\n";
                    dgvTableLexemes.RowCount = lexAnalysis.lexemes.Count;
                    for (int i = 0; i < lexAnalysis.lexemes.Count; i++)
                    {
                        dgvTableLexemes.Rows[i].Cells[0].Value = lexAnalysis.lexemes[i].Name;
                        dgvTableLexemes.Rows[i].Cells[1].Value = lexAnalysis.lexemes[i].Type;
                    }
                    foreach (Token token in lexAnalysis.tokens)
                    {
                        tbTokens.Text += $"{token}\r\n";
                    }
                    Parser parser = new Parser(lexAnalysis.tokens);
                    try
                    {
                        parser.Program();
                        tbResult.Text += "Синтаксический анализ завершён. Ошибок не обнаружено.";
                        foreach (string s in parser.stringsAnalyzedExpr)
                        {
                            tb_BZ.Text += $"{s}\r\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        tbResult.Text += "Синтаксический анализ завершён. Ошибка:\r\n-" + ex.Message;
                    }
                }
                catch(Exception ex)
                {
                    tbResult.Text += "Лексический анализ завершён. Ошибка:\r\n-" + ex.Message + "\r\n";
                }
            }
            else MessageBox.Show("Код не введен!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tbCode.Clear();
            tbResult.Clear();
            tbTokens.Clear();
            tb_BZ.Clear();
            dgvTableLexemes.Rows.Clear();
        }
    }
}
