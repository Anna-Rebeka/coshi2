using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Xml.Linq;

namespace coshi2
{
    public class Syntax
    {
        public virtual void generate()
        {
        }

    }

    public class Const : Syntax
    {
        public int value; //hodnota konštanty – typu číslo

        public Const(int nvalue)
        {
            value = nvalue;
        }

        public override void generate()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_PUSH);
            VirtualMachine.poke(value);
        }
    }

    public class RobotCommand : Syntax
    {

    }

    public class Up : RobotCommand
    {
        public Up() : base()
        {
        }

        public override void generate()
        {        //.. generuje inštrukcie pre príkaz dopredu
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_UP); //.. inštrukcia FD dĺžka
        }
    }


    public class Lt : RobotCommand
    {
        public Lt() : base()
        {
        }
        public override void generate()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_LT);
        }
    }

    public class Rt : RobotCommand
    {
        public Rt() : base()
        {
        }

        public override void generate()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_RT);
        }
    }

    public class Dw : RobotCommand
    {
        public Dw() : base()
        {
        }

        public override void generate()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_DW);
        }
    }


    public class Block : Syntax
    {
        protected List<Syntax> items = new List<Syntax>();  //... zoznam synov – prvky typu Syntax

        public Block(params Syntax[] children)     //... premenlivý počet parametrov
        {
            items.AddRange(children);   //.. zapamätáme si zoznam synov
        }

        public void add(Syntax item)    //... pridá prvok do zoznamu
        {
            items.Add(item);
        }


        public override void generate()      //... vygeneruje inštrukcie pre mašinu
        {
            foreach (Syntax item in items)
            {
                item.generate();
            }
        }
    }

    public class Repeat : Syntax
    {
        public Const count;     //premenná count...počet opakovaní – typu Const
        public Block body;      //premenná body ... telo cyklu – typu Block

        public Repeat(Const ncount, Block nbody)
        {
            count = ncount;         //.. konštruktor si zapamätá počet opakovaní
            body = nbody;           //... aj telo cyklu
        }



        public override void generate()      //... vygeneruje inštrukcie pre konštrukciu cyklu
        {
            count.generate();
            int bodyLoop = VirtualMachine.adr;
            body.generate();

            VirtualMachine.poke(VirtualMachine.INSTRUCTION_LOOP);
            VirtualMachine.poke(bodyLoop);
        }
    }

    public class UnaryOperation : Syntax
    {
        protected Syntax e;
        public UnaryOperation(Syntax ne)
        {
            e = ne;
        }
    }

    public class BinaryOperation : Syntax
    {
        protected Syntax l;
        protected Syntax r;

        public BinaryOperation(Syntax nl, Syntax nr)
        {
            l = nl;
            r = nr;
        }
    }

    public class Minus : UnaryOperation
    {

        public Minus(Syntax ne) : base(ne) { }

        public override void generate()
        {
            e.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_MINUS);
        }
    }

    public class Add : BinaryOperation
    {

        public Add(Syntax nl, Syntax nr) : base(nl, nr) { }

        public override void generate()
        {
            l.generate();
            r.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_ADD);
        }
    }


    public class Sub : BinaryOperation
    {
        public Sub(Syntax nl, Syntax nr) : base(nl, nr) { }

        public override void generate()
        {
            l.generate();
            r.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_SUB);
        }
    }



    public class Variable : Syntax
    {
        string name;
        public Variable(string nname)
        {
            name = nname;
        }

        public override void generate()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_GET);
            VirtualMachine.poke(VirtualMachine.variables[name]);
        }

        public void generateSet()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_SET);
            VirtualMachine.poke(VirtualMachine.variables[name]);
        }
    }

    public class Assign : Syntax
    {
        Variable var;
        Syntax exp;

        public Assign(Variable nvar, Syntax nexp)
        {
            var = nvar;
            exp = nexp;
        }

        public override void generate()
        {
            exp.generate();
            var.generateSet();
        }
    }


    public class While : Syntax
    {
        Syntax test;
        Syntax body;

        public While(Syntax ntest, Syntax nbody)
        {
            test = ntest;
            body = nbody;
        }

        public override void generate()
        {
            int test_adr = VirtualMachine.adr;
            test.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_JUMPIFFALSE);
            int jump_ins = VirtualMachine.adr;
            VirtualMachine.adr += 1;
            body.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_JUMP);
            VirtualMachine.poke(test_adr);
            VirtualMachine.mem[jump_ins] = VirtualMachine.adr;
        }
    }

    public class IfFalse : Syntax
    {
        Syntax test;
        Syntax bodytrue;
        Syntax bodyfalse;

        public IfFalse(Syntax ntest, Syntax nbodytrue, Syntax nbodyfalse)
        {
            test = ntest;
            bodytrue = nbodytrue;
            bodyfalse = nbodyfalse;
        }
        public override void generate()
        {
            test.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_JUMPIFFALSE);
            int jumpfalse_ins = VirtualMachine.adr;
            VirtualMachine.adr += 1;
            bodytrue.generate();
            if (bodyfalse == null)
            {
                VirtualMachine.mem[jumpfalse_ins] = VirtualMachine.adr; ;
            }
            else
            {
                VirtualMachine.poke(VirtualMachine.INSTRUCTION_JUMP);
                int jump_ins = VirtualMachine.adr;
                VirtualMachine.adr += 1;
                VirtualMachine.mem[jumpfalse_ins] = VirtualMachine.adr;
                bodyfalse.generate();
                VirtualMachine.mem[jump_ins] = VirtualMachine.adr;
            }
        }
    }

    public class Subroutine : Syntax
    {
        public string name;
        public Syntax body;
        public int bodyadr;

        public Subroutine(string nname, Syntax nbody)
        {
            name = nname;
            body = nbody;
        }

        public override void generate()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_JUMP);
            VirtualMachine.adr += 1;
            bodyadr = VirtualMachine.adr;
            body.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_RETURN);
            VirtualMachine.mem[bodyadr - 1] = VirtualMachine.adr;
        }
    }


    public class Call : Syntax
    {
        string name;
        public Call(string nname)
        {
            name = nname;
        }

        public override void generate()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_CALL);
            VirtualMachine.poke(VirtualMachine.subroutines[name].bodyadr);
        }
    }


    public class Print : Syntax
    {
        Syntax exp;

        public Print(Syntax nexp)
        {
            exp = nexp;
        }

        public override void generate()
        {
            exp.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_PRINT);
        }
    }


    public class Lower : BinaryOperation
    {

        public Lower(Syntax nl, Syntax nr) : base(nl, nr)
        {
        }

        public override void generate()
        {
            l.generate();
            r.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_LOW);
        }
    }

    public class Greater : BinaryOperation
    {

        public Greater(Syntax nl, Syntax nr) : base(nl, nr)
        {
        }

        public override void generate()
        {
            l.generate();
            r.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_GREAT);
        }
    }

    public class Equal : BinaryOperation
    {

        public Equal(Syntax nl, Syntax nr) : base(nl, nr)
        {
        }

        public override void generate()
        {
            l.generate();
            r.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_EQUAL);
        }
    }


    public class NotEqual : BinaryOperation
    {

        public NotEqual(Syntax nl, Syntax nr) : base(nl, nr)
        {
        }

        public override void generate()
        {
            l.generate();
            r.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_NOTEQUAL);
        }
    }

    public class LowEqual : BinaryOperation
    {

        public LowEqual(Syntax nl, Syntax nr) : base(nl, nr)
        {
        }

        public override void generate()
        {
            l.generate();
            r.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_LOWEQUAL);
        }
    }

    public class GreatEqual : BinaryOperation
    {

        public GreatEqual(Syntax nl, Syntax nr) : base(nl, nr)
        {
        }

        public override void generate()
        {
            l.generate();
            r.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_GREATEQUAL);
        }
    }


    public class IsSound : Syntax
    {
        public int sound;
        public IsSound(int sound_number) {
            sound = sound_number;
        }
        public override void generate()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_PUSH);
            VirtualMachine.poke(sound);
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_ISSOUND);
        }
    }


    public class IsNotSound : Syntax
    {
        public int sound;
        public IsNotSound(int sound_number)
        {
            sound = sound_number;
        }
        public override void generate()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_PUSH);
            VirtualMachine.poke(sound);
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_ISNOTSOUND);
        }
    }

    public class FreeUp : Syntax
    {
        public FreeUp() { }
        public override void generate()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_PUSH);
            VirtualMachine.poke(1);
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_FREEUP);
        }
    }

    public class FreeDown : Syntax
    {
        public FreeDown() { }
        public override void generate()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_PUSH);
            VirtualMachine.poke(1);
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_FREEDOWN);
        }
    }

    public class FreeLeft : Syntax
    {
        public FreeLeft() { }
        public override void generate()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_PUSH);
            VirtualMachine.poke(1);
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_FREELEFT);
        }
    }

    public class FreeRight : Syntax
    {
        public FreeRight() { }
        public override void generate()
        {
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_PUSH);
            VirtualMachine.poke(1);
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_FREERIGHT);
        }
    }

    public class IfElse : Syntax
    {
        public Syntax test;
        public Syntax bodytrue;
        public Syntax bodyfalse;

        public IfElse(Syntax test, Syntax body_true, Syntax body_false)
        {
            this.test = test;
            this.bodytrue = body_true;
            this.bodyfalse = body_false;
        }

        public override void generate()
        {
            test.generate();
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_JUMPIFFALSE);
            int jumpfalse_ins = VirtualMachine.adr;
            VirtualMachine.adr = VirtualMachine.adr + 1;
            bodytrue.generate();
            if (bodyfalse == null)
            {
                VirtualMachine.mem[jumpfalse_ins] = VirtualMachine.adr;
            }
            else
            {
                VirtualMachine.poke(VirtualMachine.INSTRUCTION_JUMP);
                int jump_ins = VirtualMachine.adr;
                VirtualMachine.adr = VirtualMachine.adr + 1;
                VirtualMachine.mem[jumpfalse_ins] = VirtualMachine.adr;
                bodyfalse.generate();
                VirtualMachine.mem[jump_ins] = VirtualMachine.adr;
            }
        }
    }

    public class Play : RobotCommand
    {
        public Play() : base()
        {
        }

        public override void generate()
        {        //.. generuje inštrukcie pre príkaz dopredu
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_PLAY); //.. inštrukcia FD dĺžka
        }
    }


    public class Silence : RobotCommand
    {
        public Silence() : base()
        {
        }

        public override void generate()
        {        //.. generuje inštrukcie pre príkaz dopredu
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_SILENCE); //.. inštrukcia FD dĺžka
        }
    }

    public class Loud : RobotCommand
    {
        public Loud() : base()
        {
        }

        public override void generate()
        {        //.. generuje inštrukcie pre príkaz dopredu
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_LOUD); //.. inštrukcia FD dĺžka
        }
    }
}

