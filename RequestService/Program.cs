using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

namespace wpay.RequestService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }


    public interface IActivator
    {
        string Value();
        int Some();
    }

    public class Activator1: IActivator
    {
        public string Value() => "1";
        public int Some() => 1;
    }

    public class ActivatorTest: IActivator
    {
        public string Value() => "test";
        public int Some() => 4;
    }

    public class Actix
    {
        public Actix(IActivator act) => Activator = act;

        public IActivator Activator {get;}
    }


}
