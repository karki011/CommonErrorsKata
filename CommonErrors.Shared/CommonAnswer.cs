namespace CommonErrorsKata.Shared
{
    public class CommonAnswer : IGradable
    {
        private decimal grade;
        public decimal Grade => grade;
        public void SetGrade(decimal value) => grade = value;
    }
}
