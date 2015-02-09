using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StringTokenizer;
using System.Collections;
using Sexpression;

namespace Calculator
{
    public partial class FrmCalculator : Form
    {

        //Instance variable
        private Hashtable store;

        /// <summary>
        /// Cunstructor for FrmCalculator. Does not take arguments. Creates a new instance of the class frmCalculator with the
        /// instance variable store being an empty Hashtable
        /// </summary>
        public FrmCalculator()
        {
            InitializeComponent();
            store = new Hashtable();
        }
        public void startForm()
        {
           // richTextBox1.Control = ">>";
            this.Show();
        }


        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                if (string.IsNullOrEmpty(richTextBox1.Lines.Last()))
                {
                    throw new ArgumentException("Evaulation string is empty. Enter a statement before you push Execute");
                }
                else
                {
                    label1.Text = "";
                    string str = richTextBox1.Lines.Last();
                    Tokenizer multipleStatements = new Tokenizer(str, ";");
                    Tokenizer st = new Tokenizer(str, null);
                    Sexpr d = Calculator.calculator.stm(st, store);
                    d = d.eval(store);
                    richTextBox1.AppendText(Environment.NewLine + ">>" + d.getValue().ToString() + Environment.NewLine);            
                }
                
             
                
                //For debugging purposes.

                //Constant a = new Constant(4);
                //Constant b = new Constant(5);
                //addition ad = new addition(a, b);
              // richTextBox1.AppendText(ad.getName());
            }
            catch (ArithmeticException de)
            {
                richTextBox1.AppendText(Environment.NewLine);
                label1.Text = de.ToString() + Environment.NewLine + "Something is wrong. Correct it!";
                MessageBox.Show("sdfadsdsf");
            }
            catch (ArgumentException de)
            {
                richTextBox1.AppendText(Environment.NewLine);
                label1.Text = de.ToString();
                richTextBox1.AppendText(Environment.NewLine);
            }
            catch (InvalidOperationException de)
            {
                richTextBox1.AppendText(Environment.NewLine);
                label1.Text=de.ToString();
            }
        }
    }
}
