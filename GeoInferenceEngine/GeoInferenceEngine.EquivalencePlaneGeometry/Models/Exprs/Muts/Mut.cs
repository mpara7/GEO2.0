namespace GeoInferenceEngine.Knowledges
{
    public class Mut : IComparable<Mut>
    {
        public static Action<Mut> Record;
        public int Index { get; set; }

        public static Expr operator +(Mut expr1, Mut expr2) => Expr.FromMut(expr1) + Expr.FromMut(expr2);

        public static Expr operator -(Mut expr1, Mut expr2) => Expr.FromMut(expr1) - Expr.FromMut(expr2);

        public static Expr operator *(Mut expr1, Mut expr2) => Expr.FromMut(expr1) * Expr.FromMut(expr2);

        public static Expr operator /(Mut expr1, Mut expr2) => Expr.FromMut(expr1) / Expr.FromMut(expr2);

        public static Expr operator +(Mut expr1, Expr expr2) => Expr.FromMut(expr1) + expr2;

        public static Expr operator -(Mut expr1, Expr expr2) => Expr.FromMut(expr1) - expr2;

        public static Expr operator *(Mut expr1, Expr expr2) => Expr.FromMut(expr1) * expr2;

        public static Expr operator /(Mut expr1, Expr expr2) => Expr.FromMut(expr1) / expr2;

        public Expr Add(Mut expr2) => Expr.FromMut(this) + Expr.FromMut(expr2);

        public Expr Sub(Mut expr2) => Expr.FromMut(this) - Expr.FromMut(expr2);

        public Expr Mul(Mut expr2) => Expr.FromMut(this) * Expr.FromMut(expr2);

        public Expr Div(Mut expr2) => Expr.FromMut(this) / Expr.FromMut(expr2);

        public Expr Add(Expr expr2) => Expr.FromMut(this) + expr2;

        public Expr Sub(Expr expr2) => Expr.FromMut(this) - expr2;

        public Expr Mul(Expr expr2) => Expr.FromMut(this) * expr2;

        public Expr Div(Expr expr2) => Expr.FromMut(this) / expr2;

        public Expr Pow(Expr expr2) => Expr.FromMut(this).Pow(expr2);

        public int CompareTo(Mut other)
        {
            if (other == null) return 1;
            return (int)Expr.FromMut(this).ToString().CompareTo(Expr.FromMut(other).ToString());
        }
    }
}