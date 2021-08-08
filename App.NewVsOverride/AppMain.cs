using System;

namespace ConsoleAppHelloWorld.App.NewVsOverride
{
    class AppMain
    {
        public static void Run()
        {
            SportSedanCar sportSedanCar = new();

            Console.WriteLine($"Base : HP : {((BaseCar)sportSedanCar).GetHorsePower()}");
            Console.WriteLine($"Sedan : HP : {((SedanCar)sportSedanCar).GetHorsePower()}");
            Console.WriteLine($"SportSedan : HP : {sportSedanCar.GetHorsePower()}");
        }
    }

    class BaseCar
    {
        protected int HorsePower = 300;

        public virtual int GetHorsePower()
        {
            return HorsePower;
        }
    }

    class SedanCar : BaseCar
    {
        protected int CoolerUpgrade = 30;

        public new int GetHorsePower()
        {
            return base.GetHorsePower() + CoolerUpgrade;
        }
    }

    class SportSedanCar : SedanCar
    {
        protected int TurboHorsePower = 50;
        public new int GetHorsePower()
        {
            return base.GetHorsePower() + TurboHorsePower;
        }
    }
}
