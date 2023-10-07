using System.Drawing.Printing;

namespace ZefieLib
{
    public class Bemani
    {
        /// <summary>
        /// Generates a PSun compatible eAmuse card code
        /// </summary>
        public static string EAmuseCardGen()
        {
            return "E004" + Strings.GenerateHexString(12);

        }

        public static string? GetGameName(string gamemodel)
        {
            switch (gamemodel.Substring(0,3))
            {
                case "KFC":
                    return "Sound Voltex";

                case "JDZ":
                case "KDZ":
                case "LDJ":
                    return "Beatmania IIDX";

                case "J44":
                case "K44":
                case "L44":
                    return "Jubeat";

                case "KDM":
                    return "Dance Evolution";

                case "NBT":
                    return "Beatstream";

                case "I36":
                    return "Metal Gear";

                case "KBR":
                case "LBR":
                case "MBR":
                    return "Reflec Beat";

                case "KBI":
                    return "Tenkaichi Shogikai";

                case "K39":
                case "L39":
                case "M39":
                    return "Pop'n Music";

                case "KGG":
                    return "Steel Chronicle";

                case "JGT":
                    return "Road Fighters 3D";

                case "PIX":
                    return "Museca";

                case "R66":
                    return "Bishi Bashi Channel";

                case "J32":
                case "J33":
                case "K32":
                case "K33":
                case "L32":
                case "L33":
                case "M32":
                    return "GitaDora";
 
                case "JDX":
                case "KDX":
                case "MDX":
                    return "Dance Dance Revolution";

                case "PAN":
                    return "Nostalgia";

                case "JMA":
                case "KMA":
                case "LMA":
                    return "Quiz Magic Academy";

                case "MMD":
                    return "FutureTomTom";

                case "KK9":
                    return "Mahjong Fight Club";

                case "JMP":
                    return "HELLO! Pop'n Music";

                case "KLP":
                    return "LovePlus";

                case "NSC":
                    return "Scotto";

                case "REC":
                    return "DANCERUSH";

                case "KCK":
                case "NCK":
                    return "Winning Eleven";

                case "NCG":
                    return "Otoca D'or";

                case "LA9":
                    return "Charge Machine";

                case "JC9":
                    return "Ongaku Paradise";

                case "TBS":
                    return "Busou Shinki: Armored Princess Battle Conductor";

                default:
                    return null;
            }
        }
    }
}
