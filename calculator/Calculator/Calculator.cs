using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using StringTokenizer;
using Sexpression;

namespace Calculator
{
    public class calculator
    {
        //Hashtable store = new Hashtable();
        //SipanTokenizer.Tokenizer st = new SipanTokenizer.Tokenizer();


        /// <summary>
        /// This function triggers the calculator process. It immideatly calls the "assignment function. After it returns it calls NextToken function untill it reaches the End of the the String."
        /// </summary>
        /// <param name="st"></param>
        /// <param name="store"></param>
        /// <returns></returns>






        public static Sexpr stm(Tokenizer st, Hashtable store)
        {
            Sexpr res = assignment(st, store);
            st.NextToken();
            if (st.getString().Equals(";"))
                return res;
            else
                throw new ArithmeticException("missing;");
        }

        /// <summary>
        /// This method immediatly calles the "Expression function"
        /// </summary>
        /// <param name="st"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        private static Sexpr assignment(Tokenizer st, Hashtable store)
        {
            Sexpr ans = expr(st, store);           //Calling the expr function
            st.NextToken();                         //Next token
            if (st.getString().Equals("="))         //If the next token equals "=" the function will call the next token and store the number assigned to the letter earlier recognized in the hashtable store. 
            {
                st.NextToken();
                if (!st.isNumber())
                {
                    try
                    {
                        ans = new Assignment(ans, new Variable(st.getString()));
                        st.NextToken();
                    }
                    catch
                    {
                    }
                }
                else
                {
                    throw new ArithmeticException("Obs!! endast bokstäver kan tilldelas värden");
                }
            }
            else
            {
                st.PushBack();
                return ans;
            }
            st.PushBack();
            return ans;
        }



        //Using this composition of function, that is, first expr and then term ensures that division and multiplication has higher priority over plus and minus. 

        private static Sexpr expr(Tokenizer st, Hashtable store)
        {
            Sexpr res = term(st, store);
            st.NextToken();
            while (st.isPlus() || st.isMinus())
            {
                if (st.isPlus())
                {
                    res = new addition(res, term(st,store));
                    st.NextToken();

                }
                else if (st.isMinus())
                {
                    res = new Subtraction(res, term(st, store));
                    st.NextToken();
                }
            }
            st.PushBack();
            return res;
        }


        private static Sexpr term(Tokenizer st, Hashtable store)
        {
            Sexpr res = factor(st, store);
            st.NextToken();
            while (st.isDiv() || st.isMult())
            {
                if (st.isMult())
                {
                    res = new Mult(res,factor(st, store));
                    st.NextToken();
                }
                else if (st.isDiv())
                {
                    res = new Div(res,factor(st, store));
                    st.NextToken();
                }
            }
            st.PushBack();
            return res;
        }

        private static Sexpr factor(Tokenizer st, Hashtable store)
        {
           
                Sexpr ans = prim(st, store);
                st.NextToken();

            while(st.getString() == "") 
            {
                st.NextToken();
                if (!st.isNumber())
                    ans = new Diff(ans, prim(st, store));
                else
                    throw new ArgumentException("Nothing to apply the differentiate operator on");
            }
            st.PushBack();
                return ans;
        }



        private static Sexpr prim(Tokenizer st, Hashtable store)
        {
            st.NextToken();
            Sexpr ans;
            if (st.getString().Equals("("))
            {
                ans = assignment(st, store);
                st.NextToken();    
                if (!st.getString().Equals(")"))
                {
                    throw new ArgumentException("missing )");
                }
            }
            else if (st.ToString() == "-")
            {
                ans = new Negation(prim(st, store));
                return ans;
            }
            else if (st.ToString() == "&")
            {
                ans = new Eval(prim(st,store));
                return ans;
            }

            else if (st.isNumber())
            {
                double v = st.GetNumber();
                ans = new Constant(v);
                return ans;
            }

            else if (!st.isNumber())
            {
                string id = st.getString();
                st.NextToken();
                if(st.getString()=="(")
                {
                    st.PushBack();

                    if (id.Equals("sin"))
                    {
                        ans = new Sin(prim(st, store));
                        st.NextToken();
                    }
                    else if (id.Equals("cos"))
                    {
                        ans = new Cos(prim(st, store));
                        st.NextToken();
                    }
                    else if (id.Equals("exp"))
                    {
                        ans = new Exp(prim(st, store));
                        st.NextToken();
                    }
                    else if (id.Equals("log"))
                    {
                        ans = new Log(prim(st, store));
                        st.NextToken();
                    }
                    else
                        throw new ArgumentException("Incorrect function call");
                }
            
                else 
                    ans = new Variable(id);
                    st.PushBack();
            }
            else
            {
                st.PushBack();
                throw new ArgumentException("Incorrect function call or incorrect variable assignment");
            }
            return ans;
        }
    }
}

