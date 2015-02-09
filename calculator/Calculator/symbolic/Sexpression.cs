using System;
using System.Collections;


namespace Sexpression
{

    public abstract class Sexpr
    {

        public Sexpr()
        {
        }

        public abstract Sexpr eval(Hashtable h);

        public int priority()
        {
            return 100;
        }

        public abstract String getName();

        public virtual bool isConstant()
        {
            return false;
        }

        public virtual bool isZero()
        {
            return false;
        }

        public virtual bool isOne()
        {
            return false;
        }

        public virtual double getValue()
        {
            	throw new SystemException("Endast konstanter!");
        }

        public abstract Sexpr diff(Sexpr v);

    }

    public abstract class Atom:Sexpr
    {
   
        public override String getName()
        {
        return ToString();
        }
    }

    public abstract class binOp:Sexpr
    {
        protected Sexpr left, right;

        public binOp(Sexpr l, Sexpr r)
        {
            left=l;
            right=r;
        }

        public override abstract String getName();

        public string tostring()
        {
            string result;


            if (left.priority() <= priority())
            {
                result = "(" + left + ")";

            }
            else
            {
                result =left.ToString();
            }

            if (right.priority() <= priority())
            {
                result =result + "(" + right + ")";
            }
            else
            {
                result=result + right.ToString();
            }
            return result;
        }

    }

    public abstract class MonOp:Sexpr
    {
    protected Sexpr tecken;

        public MonOp(Sexpr t):base()
        {
	        tecken=t;
        }
   
        public String toString()
        {
	        return getName()+"("+tecken+")";
        }
   
        public override abstract String getName();

}

    public abstract class Function:Sexpr
    {
        public Sexpr argument;

        public Function(Sexpr a):base()
        {
	    argument=a;
        }
   
        public String toString()
        {
	        return getName()+"("+argument+")";
        }

        public override abstract String getName();
    }

    public class addition:binOp
    {
        public addition(Sexpr l, Sexpr r):base(l,r){}

        public new int priority()
        {
            return 10;
        }

        public override string getName()
        {
            return "+";
        }

        public override Sexpr eval(Hashtable H)
        {
            Sexpr ret = Symbolic.add(left.eval(H), right.eval(H));
            return ret;
        }

        public override Sexpr diff(Sexpr var)
        {
            return Symbolic.add(left.diff(var), right.diff(var));
        }


    }
        
    public class Constant:Atom
    {

        protected double value;

            public Constant (double v)
            {
                value=v;
            }

            public string toString()
            {
                return " "+value;
            }

            public override Sexpr eval(Hashtable h)
            {
                return new Constant(value);

            }

            public override String getName(){
	            return " "+value;
            }
            
            public override bool isConstant()
            {
	            return true;
            }
            
            public override bool isZero()
            {
	            return value==0;
            }

            public override bool isOne()
            {
	            return value==1;
            }
            
            public override double getValue()
            {
	            return value;
            }
            
            public override Sexpr diff(Sexpr x)
            {
	            return new Constant(0);
            }
        }

    public class Negation : MonOp
    {
        public Negation(Sexpr t)
            : base(t)
        {
        }

        public override String getName()
        {
            return "-";
        }

        public override Sexpr eval(Hashtable h)
        {
            return Symbolic.neg(tecken.eval(h));
        }

        public override Sexpr diff(Sexpr v)
        {
            return Symbolic.neg(tecken.diff(v));
        }
    }

    public class Assignment : binOp
    {

        public Assignment(Sexpr s, Sexpr v)
            : base(s, v)
        {
        }

        public new int priority()
        {
            return 1;
        }

        public override Sexpr eval(Hashtable h)
        {
            String r = right.getName();
            Sexpr l = left.eval(h);
            h.Add(r, l);

            return l;
        }

        public override String getName()
        {
            return "=";
        }

        public override Sexpr diff(Sexpr v)
        {
            return new Constant(0);
        }

    }

    public class Symbolic{

        public static Sexpr add(Sexpr l, Sexpr r)
        {
            if (l.isConstant() && r.isConstant())
            {
                Sexpr ret = new Constant(l.getValue() + r.getValue());

                // return new Constant(l.getValue() + r.getValue());
                return ret;
            }
            else if (l.isZero())
                return r;

            else if (r.isZero())
                return l;

            else
                return new addition(l, r);
            }

        public static Sexpr sub(Sexpr l, Sexpr r)
            {
	        if(l.isConstant() && r.isConstant())
	           return new Constant(l.getValue() - r.getValue());

	        else if(l.isZero())
	            return new Negation(r);

	        else if(r.isZero())
	            return l;

	        else 
	            return new Subtraction(l,r);
	 
    }	   
	   
	    public static Sexpr mult(Sexpr l, Sexpr r)
        {
	        if(l.isConstant() && r.isConstant())
		    return new Constant(l.getValue() * r.getValue());

	        else if(l.isZero() || r.isZero())
		    return new Constant(0);

	        else if(l.isOne())
		    return r;
	        else if(r.isOne())
		    return l;

	        else 
		    return new Mult(l,r);
	    }

	    public static Sexpr div(Sexpr l, Sexpr r)
        {

	        if(r.isZero())
		    throw new CalculatorException("F�r inte dela med noll");

	        else 
            {
		    if(l.isConstant() && r.isConstant())
		        return new Constant(l.getValue() / r.getValue());

		    else if(l.isZero())
		        return new Constant(0);

		    else
		        return new Div(l,r);
	        }
	    }

        public static Sexpr sin(Sexpr arg)
        {
            if (arg.isConstant())
                return new Constant(Math.Sin(arg.getValue()));
            else
                return new Sin(arg);
        }

        public static Sexpr cos(Sexpr arg)
        {
            if (arg.isConstant())
                return new Constant(Math.Cos(arg.getValue()));
            else
                return new Cos(arg);
        }

        public static Sexpr exp(Sexpr arg)
        {
            if (arg.isConstant())
                return new Constant(Math.Exp(arg.getValue()));
            else
                return new Exp(arg);
        }

        public static Sexpr log(Sexpr arg)
        {
            if (arg.isConstant())
                return new Constant(Math.Log(arg.getValue()));
            else
                return new Log(arg);
        }

        public static Sexpr neg(Sexpr tecken)
        {
            if (tecken.isConstant())
                return new Constant(-(tecken.getValue()));
            else
                return new Negation(tecken);
        }


    }

    public class Subtraction:binOp
    {
        public Subtraction(Sexpr x, Sexpr y):base(x,y)
        {
        }

        public new int priority()
        {
	        return 10;
        }
    
        public override Sexpr eval(Hashtable h)
        {
	        return Symbolic.sub(left.eval(h), right.eval(h));
        }

        public override String getName() 
        {
	        return "-";
        }
    
        public override Sexpr diff(Sexpr v)
        {
	        return Symbolic.sub(left.diff(v), right.diff(v));
        }
    }

    public class Mult:binOp
    {
    
        public Mult(Sexpr x, Sexpr y):base(x,y)
        {
        }

        public new int priority()
        {
	        return 20;
        }
    
        public override Sexpr eval(Hashtable h)
        {
	        return Symbolic.mult(left.eval(h), right.eval(h));
        }

        public override String getName() 
        {
	    return "*";
        }

        public override Sexpr diff(Sexpr v)
        {
	        if(left.isConstant() && right.isConstant())
	            return new Constant(0);

	            else
	            return Symbolic.add(Symbolic.mult(left.diff(v), right),
				        Symbolic.mult(left, right.diff(v)));
				
        }
    }

    public class Div:binOp
    {
    
        public Div(Sexpr x, Sexpr y):base(x,y)   
        {
        }

        public new int priority()
        {
	        return 20;
        }
    
        public override Sexpr eval(Hashtable h)
        {
	    return Symbolic.div(left.eval(h), right.eval(h));
        }

        public override String getName() {
	    return "/";
        }

        public override Sexpr diff(Sexpr v)
        {
	        if(left.isConstant() && right.isConstant())
	            return new Constant(0);
	
	        else
	            return Symbolic.div(Symbolic.sub((
				       Symbolic.mult(left.diff(v),right)),
				       Symbolic.mult(left,right.diff(v))),
				       Symbolic.mult(right,right));
    }
    
}

    public class Cos:Function
    {

        public Cos(Sexpr a):base(a)
        {
        }

        public override String getName()
        {
	    return "cos";
        }

        public override Sexpr eval(Hashtable h)
        {
	        return Symbolic.cos(argument.eval(h));
	    }

        public override Sexpr diff(Sexpr h)
        {
	    return Symbolic.mult(argument.diff(h),new Negation(new Sin(argument)));
        }

        }

    public class Diff:binOp{

        public Diff(Sexpr x, Sexpr y):base(x,y)
        {
        }

        public new int priority()
        {
	    return 15;
        }

        public override String getName()
        {
	    return "'";
        }

        public override Sexpr eval(Hashtable h)
        {
	    return left.diff(right);
        }

        public override Sexpr diff(Sexpr v)
        {
	    return new Constant(0);
        }
}

    public class Eval:MonOp{

        public Eval(Sexpr t):base(t)
        {
        }

        public override String getName(){
	    return "&";
        }

        public override Sexpr eval(Hashtable h)
        {
	    return tecken.eval(h).eval(h);
        }

        public override Sexpr diff(Sexpr v)
        {
	    return tecken.diff(v);
        }
}

    public class Exp:Function{

        public Exp(Sexpr a):base(a)
        {
        }

        public override String getName()
        {
	        return "exp";
        }

        public override Sexpr eval(Hashtable h)
        {
	        return Symbolic.exp(argument.eval(h));
	    }

        public override Sexpr diff(Sexpr v)
        {
	        return Symbolic.mult(argument.diff(v), new Exp(argument));
        }
}

    public class Log:Function
    {

        public Log(Sexpr a):base(a)
        {
        }

        public override String getName()
        {
	    return "log";
        }

        public override Sexpr eval(Hashtable h)
        {
            Sexpr v = argument.eval(h);
           // if (v.isConstant() && v.getValue() <= 0)
               // throw 
               //     new EvalException("Argumentet m�ste vara st�rre �n 1");
	    //else
                return Symbolic.log(v);
            }

        public override Sexpr diff(Sexpr v)
        {
	        return Symbolic.mult(argument.diff(v), new Div(new Constant(1), argument));
        }

    }

    public class Quot:MonOp
    {

        public Quot(Sexpr t):base(t)
        {
        }

        public override String getName()
        {
	    return "\"";
        }

        public override Sexpr eval(Hashtable h)
        {
	        return tecken.eval(new Hashtable());
        }

        public override Sexpr diff( Sexpr v)
        {
	    return tecken.diff(v);
        }
    }

    public class Sin:Function
    {

        public Sin(Sexpr a):base(a)
        {
        }

        public override String getName()
        {
	    return "sin";
        }

	    public override Sexpr eval(Hashtable h)
        {
	        return Symbolic.sin(argument.eval(h));
	    }
    
        public override Sexpr diff(Sexpr h)
        {
	        return Symbolic.mult(argument.diff(h),
                               new Cos(argument));
        }

    }

    public class Variable:Atom
    {  
        protected String name;

        public Variable (String n):base()
        {
	    name=n;
        }

        public override String ToString()
        {
	        return name;
        }

        public override Sexpr eval(Hashtable h)
        {       
            Sexpr v =(Sexpr)h[name];                     	        
	        if(v==null)
	            return new Variable(name);
	        else
	            return v;
        }

        public override String getName()
        {
	        return name;
        }
    
        public override Sexpr diff(Sexpr v)
        {
	        if(name.Equals(v.ToString()))
	            return new Constant(1);
	        else
	            return new Constant(0);
        }
    }

}