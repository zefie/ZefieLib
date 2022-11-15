using System.Drawing.Printing;

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

        public static string getGameName(string gamemodel)
        {
            switch (gamemodel.Substring(0,3))
            {
                case "KFC":
                    return "Sound Voltex";
                    break;

                case "JDZ":
                case "KDZ":
                case "LDJ":
                    return "Beatmania IIDX";
                    break;

                case "J44":
                case "K44":
                case "L44":
                    return "Jubeat";
                    break;

                case "KDM":
                    return "Dance Evolution";
                    break;

                case "NBT":
                    return "Beatstream";
                    break;

                case "I36":
                    return "Metal Gear";
                    break;

                case "KBR":
                case "LBR":
                case "MBR":
                    return "Reflec Beat";
                    break;

                case "KBI":
                    return "Tenkaichi Shogikai";
                    break;

                case "K39":
                case "L39":
                case "M39":
                    return "Pop'n Music";
                    break;

                case "KGG":
                    return "Steel Chronicle";
                    break;

                case "JGT":
                    return "Road Fighters 3D";
                    break;

                case "PIX":
                    return "Museca";
                    break;

                case "R66":
                    return "Bishi Bashi Channel";
                    break;


                case "J32":
                case "J33":
                case "K32":
                case "K33":
                case "L32":
                case "L33":
                case "M32":
                    return "GitaDora";
                    break;

                case "JDX":
                case "KDX":
                case "MDX":
                    return "Dance Dance Revolution";
                    break;

                case "PAN":
                    return "Nostalgia";
                    break;

                case "JMA":
                case "KMA":
                case "LMA":
                    return "Quiz Magic Academy";
                    break;

                case "MMD":
                    return "FutureTomTom";
                    break;

                case "KK9":
                    return "Mahjong Fight Club";
                    break;

                case "JMP":
                    return "HELLO! Pop'n Music";
                    break;

                case "KLP":
                    return "LovePlus";
                    break;

                case "NSC":
                    return "Scotto";
                    break;

                case "REC":
                    return "DANCERUSH";
                    break;

                case "KCK":
                case "NCK":
                    return "Winning Eleven";
                    break;

                case "NCG":
                    return "Otoca D'or";
                    break;

                case "LA9":
                    return "Charge Machine";
                    break;

                case "JC9":
                    return "Ongaku Paradise";
                    break;

                case "TBS":
                    return "Busou Shinki: Armored Princess Battle Conductor";
                    break;

                default:
                    return null;
            }
        }
    }
}
