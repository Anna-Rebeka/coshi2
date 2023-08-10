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
        public VirtualMachine virtualMachine;

        public Const(int nvalue, VirtualMachine vm)
        {
            value = nvalue;
            virtualMachine = vm;
        }

        public override void generate()
        {
            virtualMachine.poke(value);
        }
    }

    public class RobotCommand : Syntax
    {
        public VirtualMachine virtualMachine;

        public RobotCommand(VirtualMachine vm)
        {
            virtualMachine = vm;
        }
    }

    public class Up : RobotCommand
    {
        public Up(VirtualMachine vm) : base(vm)
        {
        }

        public override void generate()
        {        //.. generuje inštrukcie pre príkaz dopredu
            virtualMachine.poke(virtualMachine.INSTRUCTION_UP); //.. inštrukcia FD dĺžka
        }
    }


    public class Lt : RobotCommand
    {
        public Lt(VirtualMachine vm) : base(vm)
        {
        }


        public override void generate()
        {
            virtualMachine.poke(virtualMachine.INSTRUCTION_LT);
        }
    }

    public class Rt : RobotCommand
    {
        public Rt(VirtualMachine vm) : base(vm)
        {
        }


        public override void generate()
        {
            virtualMachine.poke(virtualMachine.INSTRUCTION_RT);
        }
    }

    public class Dw : RobotCommand
    {
        public Dw(VirtualMachine vm) : base(vm)
        {
        }

        public override void generate()
        {
            virtualMachine.poke(virtualMachine.INSTRUCTION_DW);
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
        public VirtualMachine virtualMachine;

        public Repeat(Const ncount, Block nbody, VirtualMachine vm)
        {
            count = ncount;         //.. konštruktor si zapamätá počet opakovaní
            body = nbody;           //... aj telo cyklu
            virtualMachine = vm;
        }

 

        public override void generate()      //... vygeneruje inštrukcie pre konštrukciu cyklu
        {
            virtualMachine.poke(virtualMachine.INSTRUCTION_SET);    //... inštrukcia pre nastavenie počítadla
            virtualMachine.poke(counter_adr);
            count.generate();
            counter_adr--;
            int loop_body = virtualMachine.adr;     //... zapamätáme si začiatok tela cyklu
            body.generate();                        //... vygenerujú sa inštrukcie pre telo cyklu
            counter_adr++;
            virtualMachine.poke(virtualMachine.INSTRUCTION_LOOP);   //... inštrukcia pre zopakovanie tela cyklu
            virtualMachine.poke(counter_adr);
            virtualMachine.poke(loop_body);
        }
    }
}

