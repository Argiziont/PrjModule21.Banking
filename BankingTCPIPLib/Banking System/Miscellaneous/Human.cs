using System;

namespace BankingTCPIPLib.Banking_System.Miscellaneous
{
    [Serializable]
    public class Human
    {
        public Human()
        {
        }

        public Human(string name, string surname, string lastName, DateTime birthDate, HumanSex sex)
        {
            Name = name;
            Surname = surname;
            LastName = lastName;
            BirthDate = birthDate;
            Sex = sex;
        }

        public string Name { get; init; }
        public string Surname { get; init; }
        public string LastName { get; init; }
        public DateTime BirthDate { get; init; }
        public HumanSex Sex { get; init; }
    }
}