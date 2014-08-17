namespace Zefie
{
    public class Bemani
    {
        /// <summary>
        /// Generates a PSun compatible eAmuse card code
        /// </summary>
        public static string eAmuseCardGen()
        {
            return "E004" + Strings.genHexString(12);
        }
    }
}
