using System;

namespace ZennoSite.Utils
{
    public static class SerialKey
    {
        private static Random r = new Random();
        private static string numbers = "0123456789";
        private static string words = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string Generate()
        {
            return GetFirstSection() + "-" + GetSecondSection() + "-" + GetThirdSection();
        }

        private static string GetFirstSection()
        {
            return GetRandomNumber() + GetRandomWord() + GetRandomNumber() + GetRandomWord();
        }

        private static string GetSecondSection()
        {
            var section = "";

            var character = GetRandomCharacter();
            var doubleCharacter = character + character;

            switch (r.Next(0, 3))
            {
                case 0:
                    section = doubleCharacter + GetRandomCharacter() + GetRandomCharacter();
                    break;
                case 1:
                    section = GetRandomCharacter() + doubleCharacter + GetRandomCharacter();
                    break;
                case 2:
                    section = GetRandomCharacter() + GetRandomCharacter() + doubleCharacter;
                    break;
            }

            return section;
        }

        private static string GetRandomNumber() => numbers[r.Next(numbers.Length)].ToString();
        private static string GetRandomWord() => words[r.Next(words.Length)].ToString();
        private static string GetRandomCharacter()
        {
            if (r.Next(0, 2) > 0)
            {
                return GetRandomNumber();
            }

            return GetRandomWord();
        }

        private static string GetThirdSection()
        {
            return GetRandomNumber() + GetRandomCharacter() + GetRandomCharacter() + GetRandomCharacter();
        }
    }
}
