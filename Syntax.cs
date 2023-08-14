using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;

namespace coshi2
{
    public class Syntax
    {
        public int counter_adr;

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

    public class Repeat: Syntax
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
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_SET);    //... inštrukcia pre nastavenie počítadla
            VirtualMachine.poke(counter_adr);
            count.generate();
            counter_adr--;
            int loop_body = VirtualMachine.adr;     //... zapamätáme si začiatok tela cyklu
            body.generate();                        //... vygenerujú sa inštrukcie pre telo cyklu
            counter_adr++;
            VirtualMachine.poke(VirtualMachine.INSTRUCTION_LOOP);   //... inštrukcia pre zopakovanie tela cyklu
            VirtualMachine.poke(counter_adr);
            VirtualMachine.poke(loop_body);
        }
    }
}

