using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Net;
using System.Threading;
using System.IO;

namespace Parser
{
    class Programт
    {
        /// <summary>
        /// Путь для хранения.
        /// </summary>
        private static string Path;
        /// <summary>
        /// Том манги.
        /// </summary>
        private static int Tom;
        /// <summary>
        /// Глава манги.
        /// </summary>
        private static int Glav;
        /// <summary>
        /// Сколько глав надо скачать.
        /// </summary>
        private static int GlavNado;
        /// <summary>
        /// Название манги.
        /// </summary>
        private static string Name;
        /// <summary>
        /// Имя последнего файла.
        /// </summary>
        private static string LastFile;
        /// <summary>
        /// Ссылка на последний файл.
        /// </summary>
        private static string LinkFile ="";


        private static string[] Logo = {"███╗   ███╗ █████╗ ███╗  ██╗ ██████╗  █████╗ ",
                                        "████╗ ████║██╔══██╗████╗ ██║██╔════╝ ██╔══██╗",
                                        "██╔████╔██║███████║██╔██╗██║██║  ██╗ ███████║",
                                        "██║╚██╔╝██║██╔══██║██║╚████║██║  ╚██╗██╔══██║",
                                        "██║ ╚═╝ ██║██║  ██║██║ ╚███║╚██████╔╝██║  ██║",
                                        "╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚══╝ ╚═════╝ ╚═╝  ╚═╝",
                                        "                                             ",
                                        "██████╗  █████╗ ██████╗  ██████╗███████╗██████╗",
                                        "██╔══██╗██╔══██╗██╔══██╗██╔════╝██╔════╝██╔══██╗",
                                        "██████╔╝███████║██████╔╝╚█████╗ █████╗  ██████╔╝",
                                        "██╔═══╝ ██╔══██║██╔══██╗ ╚═══██╗██╔══╝  ██╔══██╗",
                                        "██║     ██║  ██║██║  ██║██████╔╝███████╗██║  ██║",
                                        "╚═╝     ╚═╝  ╚═╝╚═╝  ╚═╝╚═════╝ ╚══════╝╚═╝  ╚═╝"};

        static void Main(string[] args)
        {
            ShowLogo();
            Dialog();

            Console.WriteLine("Нажмите любую клавишу, что выйти......");
            Console.ReadKey();
        }

        /// <summary>
        /// UI
        /// </summary>
        private static void ShowLogo()
        {
            Console.Title = "Manga Parser";

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            foreach (var item in Logo)
            {
                Console.Write("                                   ");
                Console.WriteLine(item);
                
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// Ожидание загрузки файла.
        /// </summary>
        private static void FinishidDomlodal()
        {
            FileInfo fl = new FileInfo(LastFile);
            int lengNow = 0;
            int ling = int.Parse(GetFileSize(new Uri(LinkFile))) / 1024 ; ;
            int cursPos = Console.CursorTop;

            do
            {
                fl = new FileInfo(LastFile);
                lengNow = int.Parse((fl.Length/1024).ToString());
                Console.SetCursorPosition(0, cursPos);
                Console.CursorVisible = false;
                Console.Write(" Загруженно "+lengNow+"Kb из " + ling+"Kb");

            } while (ling != lengNow);
            Console.CursorVisible = true;
            Console.WriteLine();
        }

        /// <summary>
        /// Диалог с пользователем.
        /// </summary>
        private static void Dialog()
        {
            Console.Write("Введите ссылку на мангу: ");
            string s = Console.ReadLine();
            
            if(string.IsNullOrWhiteSpace(s))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ссылка не может быть пустой...");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if(!(new Regex(@"^(https?:\/\/)?([\w-]{1,32}\.[\w-]{1,32})[^\s@]*")).IsMatch(s))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Не корректная ссылка...");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.Write("Введите кол-во глав: ");
            try
            {
                GlavNado = int.Parse(Console.ReadLine());
            }
            catch 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Не корректное значение глав...");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            
            if(GlavNado <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Не корректное значение глав...");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.Write("Введите путь хранения: ");
            Path = Console.ReadLine();

            if(string.IsNullOrWhiteSpace(Path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Путь для хранения не может быть пустым...");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (!Path.EndsWith("\\"))
                Path += "\\";

            string[] arrayInfo = s.Split('/');

            Tom = int.Parse(arrayInfo[4]);
            Glav = int.Parse(arrayInfo[5]);
            Name = arrayInfo[3];

            Enumeration();

            Thread.Sleep(500);
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine("Ожидайте окончательной загрузки");
            //Console.ForegroundColor = ConsoleColor.White;
            //FinishidDomlodal();
        }

        /// <summary>
        /// Перебор файлов.
        /// </summary>
        private static void Enumeration()
        {
            for (int glav = Glav; glav < GlavNado+Glav; glav++)
            {
                string link = "https://manga-online.biz/" + Name + "/" + Tom + "/" + glav + "/" + 1 + "/";
                string html = HtmlPack(link);
                string linkfile = GetHref(html);
                LinkFile = linkfile;
                Console.WriteLine(LinkFile);

                if (linkfile == "" || System.IO.Path.GetExtension(linkfile) != ".zip")
                {
                    Tom++;
                    glav--;
                }
                else
                {
                    DomlodalFile(linkfile, Path + Name + "_" + Tom + "_" + glav+".zip");
                    LastFile = Path + Name + "_" + Tom + "_" + glav + ".zip";
                    FinishidDomlodal();
                }
            }
        }

        /// <summary>
        /// Получить код страницы.
        /// </summary>
        /// <param name="linkSite"></param>
        /// <returns></returns>
        public static string HtmlPack(string linkSite)
        {
            HtmlWeb doc = new HtmlWeb();

            HtmlDocument htmlSnippet = doc.Load(linkSite);

            return htmlSnippet.ParsedText;
        }

        /// <summary>
        /// Скачиваниие файла.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="fileName"></param>
        private static void DomlodalFile(string link, string fileName)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFileAsync(new Uri(link), fileName);
        }

        /// <summary>
        /// Получение ссылки для на архив.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetHref(string input)
        {
            //string res = "";

            //Regex reg = new Regex(@"\w+:[\/]*\w+[.](manga-online.biz)[\/]\w+[\/]\w+[\/]\w+[\/]\w+[.]\w+");
            //Regex reg2 = new Regex(@"(https)[:][\/]+\w+[.](manga-online.biz)[\/]\w+[\/]\w+[\/]\w+[\/]\w+[\/]\w+[.]\w+");

            //if(reg.IsMatch(input))
            //{
            //    MatchCollection mt = reg.Matches(input);

            //    foreach (var item in mt)
            //    {
            //        res = item.ToString();
            //    }
            //}
            //if(reg2.IsMatch(input))
            //{
            //    MatchCollection mt = reg2.Matches(input);

            //    foreach (var item in mt)
            //    {
            //        res = item.ToString();
            //    }
            //}

            Regex reg = new Regex(@"(https)[:][\/]+\w+[.](manga-online.biz)[\/]\w+[\/]\w+[\/]\w+[-]\w+[.](zip)");

            return reg.Match(input).Value.ToString();
        }

        /// <summary>
        /// Размер скачиваемого файла.
        /// </summary>
        /// <param name="uriPath"></param>
        /// <returns></returns>
        private static string GetFileSize(Uri uriPath)
        {
            var webRequest = HttpWebRequest.Create(uriPath);
            webRequest.Method = "HEAD";

            using (var webResponse = webRequest.GetResponse())
            {
                return  webResponse.Headers.Get("Content-Length");
            }
        }

    }
}
