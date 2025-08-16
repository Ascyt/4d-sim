using System;

public class NormalRandom
{
    private static NormalRandom _instance;
    public static NormalRandom Instance
    {
        get
        {
            if (_instance == null)
                _instance = new NormalRandom();

            return _instance;
        }
    }

    private System.Random random = new System.Random();

    public double NextDouble(double mu, double sigma)
    {
        double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
            System.Math.Sin(2.0 * System.Math.PI * u2); //random normal(0,1)   
        double randNormal = mu + sigma * randStdNormal; //random normal(mean,stdDev^2)
        return randNormal;
    }

    public double NextDouble(double mu, double sigma, double min, double max)
    {
        double result;
        do
        {
            result = NextDouble(mu, sigma);
        } while (result < min || result > max);

        return result;
    }

    public T GetRandomItem<T>(T[] values, double mu, double sigma)
    {
        if (values.Length == 0)
            throw new ArgumentException("Values array cannot be empty.", nameof(values));

        int choice;

        if (mu == 0.0) // Save 50% of calculations when using mu == 0 by using absolute 
        {
            do
            {
                choice = Math.Abs((int)NextDouble(mu, sigma));
            } while (choice >= values.Length);

            return values[choice];
        }


        choice = (int)NextDouble(mu, sigma, 0, values.Length);

        if (choice == values.Length) // Just to make sure that doesn't happen
            choice--;

        return values[choice];
    }
}