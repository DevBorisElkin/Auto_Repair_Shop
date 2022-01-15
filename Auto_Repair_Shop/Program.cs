﻿using ClassLibrary1;
using System;

namespace Auto_Repair_Shop
{
    class Program
    {
        static void Main(string[] args)
        {
            UserInput.GetScenarioDataByUserInput();
            new TaskImplementation();
        }
    }

    /*
        Описание задания: 

        Консольное приложение на C# .net 5 или 6

        Задача сделать симуляцию системы обслуживания клиентов в автосалоне. Вывод сообщений через консоль.
        
        100 000 Покупателей приходят в магазин автомобилей.
        К новому покупателю подходит один из 4 продавцов и вместе они формируют заявку на автомобиль за время tЗаявка.
        Далее по единственной пневмотрубе заявка отправляется в мастерскую.
        Клиент уходит в пункт выдачи за время tКлиент
        Один из 5 свободных механиков забирает заявку и делает машину за время tПроизводство.
        По готовности механик передает машину в пункт выдачи.
        В пункте выдачи клиенты получают свои машины по готовности, и уезжают из магазина.
        
        Программа не выключится, пока не нажать enter, даже если клиенты закончились.
        
        
        Условия: если время процесса не указано - процесс выполняется мгновенно.
        Надо добиться оптимальной скорости выполнения всего процесса при минимальных расходах на CPU.
        
        Добиться оптимальной работы при разных условиях:
        1) tЗаявка = 0, tКлиент = 0, tПроизводство = 0
        2) tЗаявка = 1мс, tКлиент = 2мс, tПроизводство = 0
        3) tЗаявка = 0, tКлиент = 0, tПроизводство = 1
        4) tЗаявка = 0, tКлиент = 1, tПроизводство = 1
        5) tЗаявка = 1, tКлиент = 0, tПроизводство = 1
*/
}