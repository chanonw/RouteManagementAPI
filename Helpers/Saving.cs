namespace RouteAPI.Helpers
{
    public class Saving
    {
        private double saving;
        private Route2 r1;
        private Route2 r2;

        public double Savings { get => saving; set => saving = value; }

        public Saving(double saving, Route2 r1, Route2 r2)
        {
            this.Savings = saving;
            this.r1 = r1;
            this.r2 = r2;
        }
        public double getSaving()
        {
            return this.Savings;
        }

        public void setSaving(double saving)
        {
            this.Savings = saving;
        }

        public Route2 getR1()
        {
            return this.r1;
        }

        public void setR1(Route2 r1)
        {
            this.r1 = r1;
        }

        public Route2 getR2()
        {
            return this.r2;
        }

        public void setR2(Route2 r2)
        {
            this.r2 = r2;
        }
    }
}