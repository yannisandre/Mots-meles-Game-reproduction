using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotMeles
{
    public class Plateau
    {

        private string[,] matrice; /// matrice de chaines de caractère qui contient les lettres de la grille (les chaines de caractères ont été priorisées
                                   /// par rapport aux char car certaines méthodes intéréssantes des string ne sont pas valables sur les chars
        private int difficulte; /// difficulte de la grille (allant de 1 à 5)
        private List<string> mot_recherches; /// liste des mots à rechercher sur la grille
        private Dictionnaire dico; /// dictionnaire contenant les mots de la grille 
        private List<(int, int)> liste_lettres_vertes; /// collection de type liste de tuple d'entier qui contient les coordonnées (ligne,colonne)
                                                       /// des lettres de la grille des mots déjà trouvé afin de les afficher en vert par la suite
        private int colonne; /// les attributs ligne et colonne n'ont besoin d'être spécifié que lorsque la grille est générée aléatoirement, en effet
        private int ligne;   /// les fichiers csv contiennent déjà ces informations
        private string langue;
        private int nombre_mots;

        public Plateau(string nom_grille, string aleatoire,Dictionnaire dico, int ligne, int colonne, int difficulte, int nombre_mots)
        /// Constructeur des grille de mot-meles
        {
            this.mot_recherches = new List<string>();
            this.liste_lettres_vertes = new List<(int, int)>();
            this.dico = dico;
            this.langue = dico.Langue;
            
            if (aleatoire == "yes")
            {
                this.nombre_mots = nombre_mots;
                this.difficulte = difficulte;
                this.ligne = ligne;
                this.colonne = colonne;
                this.matrice = GenerateurGrille(ligne, colonne, difficulte, langue, nombre_mots);
            }
            else
            {
                this.nombre_mots = 0;
                /// initialise la grille à partir d'un fichier csv
                this.ToRead(nom_grille);
                
            } 
        }

        public string toString()
        /// Cette méthode permet d'afficher la grille courante de motmele
        {
            string chaine = "";
            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    chaine += matrice[i, j] + " ";
                }
                chaine += "\n";
            }
            return chaine;
        }

        public void toString_ameliore()
        /* Cette méthode permet d'afficher la grille avec les lettres des mots qui ont déjà été trouvés colorées en vert avec une petite mise en forme graphique
         comprenant également l'affichage des mots à trouver à droite de la grille
        */
        {
            string[,] mat_affiche = mots_a_trouver_en_colonne(); /// récupère les mots à rechercher dans une matrice de sorte à les afficher en colonne
            Console.WriteLine();
            /// toute la partie suivante est une msie en forme graphique en alternant la couleur d'affichage de la console
            Console.Write("                                 ");
            for (int i = 1; i < matrice.GetLength(1) + 1; i++) { decor_noel(i); if (i < 9) { Console.Write(i + " "); } else { Console.Write(i); } } 
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                               ╔");
            for (int i = 0; i < matrice.GetLength(1) * 2 + 1; i++) { decor_noel(i); Console.Write("═"); }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╗");
            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                decor_noel(i);
                if (i+1 < 10) { Console.Write("                             " + (i + 1) + " ║ "); }
                else { Console.Write("                            " + (i + 1) + " ║ "); }
                
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    if ( liste_lettres_vertes.Contains( (i,j) ) )
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(matrice[i, j] + " ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else { decor_noel(i); Console.Write(matrice[i, j] + " "); }
                }
                Console.Write("║");
                
                
                for (int k = 0; k < mat_affiche.GetLength(1);k++)
                {
                    if (mat_affiche[i,k] != null && mat_affiche[i, k] != "rien") { Console.Write(" " + mat_affiche[i, k] + " "); }
                }
                
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("                               ╚");
            for (int i = 0; i < matrice.GetLength(1)*2+1; i++) { decor_noel(i); Console.Write("═"); }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╝");
        }

        public void ToRead(string nomfile)
        /// Cette fonction permet de générer une grille à partir d'un fichier csv
        {
            string[] lines = File.ReadAllLines(nomfile + ".csv");
            string[] premiere_ligne = lines[0].Split(';'); /// tableau contenant la première ligne du fichier
            this.difficulte = Convert.ToInt32(premiere_ligne[0]); /// on récupère le niveau (premier caractère de la première ligne) et on le convertit en nombre entier
            this.matrice = new string[Convert.ToInt32(premiere_ligne[1]), Convert.ToInt32(premiere_ligne[2])]; // on initialise la taille de la grille avec le nb de lignes et de colonnes
            
            /// cette double boucle permet de remplir la grille avec les lettres présentes dans le fichier csv
            for (int i = 2; i < matrice.GetLength(0) + 2; i++)
            {
                string[] test = lines[i].Split(';');
                for (int j = 0; j < matrice.GetLength(1); j++)
                {

                    matrice[i - 2, j] = test[j];
                    
                }
            }
            string[] deuxieme_ligne = lines[1].Split(';'); ///  deuxième ligne du fichier csv contenant les mots recherchés
            foreach (string e in deuxieme_ligne) { mot_recherches.Add(e);} /// on les ajoute à la liste des mots à trouver
        }

        
        public void ToFile(string nomfile)
        /// Cette fonction permet de sauvegarder dans un fichier les informations d'une grille sous le format habituel en csv
        {
            string[] lignes = new string[matrice.GetLength(0)+2]; /// on veut écrire autant de lignes que la matrice en possède ainsi que deux autres (dimensions,mots restants à trouver)
            lignes[0] = difficulte + ";" + matrice.GetLength(0) + ";" + matrice.GetLength(1) + ";" + mot_recherches.Count + ";";
            string chaine2 = "";
            int k = 0;
            foreach(string mot in mot_recherches) { if (k != mot_recherches.Count - 1) { chaine2 += mot + ";"; } else { chaine2 += mot; } k++; }
            lignes[1] = chaine2; ///  la deuxième ligne contient les mots qui n'ont pas encore été trouvés sur la grille
            /// On ajoute toutes les lignes de la grille
            for(int i = 0; i < matrice.GetLength(0); i++)
            {
                string chaine = "";
                for(int j = 0; j < matrice.GetLength(1); j++)
                {
                    chaine += matrice[i,j] + ";";
                }
                lignes[i + 2] = chaine;
            }
            File.WriteAllLines(nomfile + ".csv", lignes); /// finalement on écrit le tout sur le fichier csv
        }
        

        public bool Test_Plateau(string mot, int ligne, int colonne, string direction)
        /// Cette fonction permet de tester si un emplacement et une direction choisies par le joueur correspondent ou non à un mot de la grille à trouver.
        {
            if (ligne < 1 || ligne > matrice.GetLength(0) || colonne < 1 || colonne > matrice.GetLength(1)) { Console.WriteLine("Veuillez une ligne et une colonne valides"); return false; }
            mot = mot.ToLower(); /// on convertit le mot en minuscule afin d'éviter tout problème (majuscule/minuscule).
            if (mot.Length < 2 || mot.Length > 15) { Console.WriteLine("taille de mot invalide"); return false; }
            if ( !(dico.RechDichoRecursif(mot, dico.nombre_mots_n_lettres(mot.Length)) ) ) { Console.WriteLine("Le mot n'est pas présent dans le dictionnaire " + dico.Langue); return false; }
            int test = 0;
            foreach (string mots in mot_recherches) 
            { 
                if (mots == mot.ToUpper()) { test = 1; break; }
            }
            /// si test = 1 cela signifie qu'il existe dans la liste des mots à rechercher
            if (test == 0) { Console.WriteLine("Le mot n'est pas dans la liste des mots recherchés"); return false; }
            List<(int, int)> liste_lettres_temp = new List<(int, int)>(); /// on initialise la liste des tuples des lettres à mettre en vert dans l'affichage.

            /// Ici on réduit de 1 la ligne et la colonne car les indices dans la matrice commencent à 0
            ligne -= 1; 
            colonne -= 1;
            
            /// dans les 4 cas suivants on va parcourir la grille en fonction de la direction afin de voir si le mot s'y trouve ou non
            
            if (direction.ToLower() == "e" || direction.ToLower() == "o") /// cas du choix des lignes horizontales.
            {
                    int indice_mot = 0;
                    int bouger = 0;
                    if (direction.ToLower() == "e") { bouger = 1; }
                    else { bouger = -1; }
                    while (colonne < matrice.GetLength(1) && colonne >= 0 && indice_mot != mot.Length-1)
                    {
                        if (matrice[ligne, colonne].ToLower() != Convert.ToString(mot[indice_mot])) { Console.WriteLine("Le mot ne se trouve pas içi !"); liste_lettres_temp.Clear(); return false; }
                        liste_lettres_temp.Add((ligne, colonne));
                        indice_mot++;
                        colonne+=bouger;
                    }
                    if (indice_mot != mot.Length - 1) { Console.WriteLine("Le mot ne se trouve pas içi !"); return false; } /// le parcours est moins le long que le nombre de lettre -> le mot ne se trouve pas ici
                    liste_lettres_temp.Add((ligne, colonne));
                    foreach ((int, int) coords in liste_lettres_temp) { if (!(liste_lettres_vertes.Contains(coords))) { liste_lettres_vertes.Add(coords); } }
                    return true; 
            }

            if (direction.ToLower() == "s" || direction.ToLower() == "n") /// cas du choix des lignes verticales
            {
                    int indice_mot = 0;
                    int bouger = 0;
                    if (direction.ToLower() == "s") { bouger = 1; }
                    else { bouger = -1; }
                    
                    while (ligne < matrice.GetLength(0) && ligne >= 0 && indice_mot != mot.Length - 1)
                    {
                        if (matrice[ligne, colonne].ToLower() != Convert.ToString(mot[indice_mot])) { Console.WriteLine("Le mot ne se trouve pas içi !"); liste_lettres_temp.Clear(); return false; }
                        liste_lettres_temp.Add((ligne, colonne));
                        indice_mot++;
                        ligne += bouger;
                        
                    }
                    if (indice_mot != mot.Length-1) { Console.WriteLine("Le mot ne se trouve pas içi !"); return false; } /// le parcours est moins le long que le nombre de lettre -> le mot ne se trouve pas ici
                    liste_lettres_temp.Add((ligne, colonne));
                    foreach ((int, int) coords in liste_lettres_temp) { if (!(liste_lettres_vertes.Contains(coords))) { liste_lettres_vertes.Add(coords); } }
                    return true;
            }

            if (direction.ToLower() == "ne" || direction.ToLower() == "so") /// cas du choix de la diagonale ascendante dans les deux sens
            {
                    int indice_mot = 0;
                    int bouger_lignes = 0;
                    int bouger_colonnes = 0;
                    if (direction.ToLower() == "ne") { bouger_lignes = -1; bouger_colonnes = 1; }
                    else { bouger_lignes = 1; bouger_colonnes = -1; }
                    while (ligne < matrice.GetLength(0) && ligne >= 0 && colonne < matrice.GetLength(1) && colonne >= 0 && indice_mot != mot.Length - 1)
                    {
                        if (matrice[ligne, colonne].ToLower() != Convert.ToString(mot[indice_mot])) { Console.WriteLine("Le mot ne se trouve pas içi !"); liste_lettres_temp.Clear(); return false; }
                        indice_mot++;
                        liste_lettres_temp.Add((ligne, colonne));
                        ligne += bouger_lignes;
                        colonne += bouger_colonnes;
                    }
                    if (indice_mot != mot.Length - 1) { Console.WriteLine("Le mot ne se trouve pas içi !"); return false; } /// le parcours est moins le long que le nombre de lettre -> le mot ne se trouve pas ici
                    liste_lettres_temp.Add((ligne, colonne));
                    foreach ((int, int) coords in liste_lettres_temp) { if (!(liste_lettres_vertes.Contains(coords))) { liste_lettres_vertes.Add(coords); } }
                    return true; 
            }
            if (direction.ToLower() == "se" || direction.ToLower() == "no") /// cas du choix de la diagonale descendante dans les deux sens
            {
                    int indice_mot = 0;
                    int bouger_lignes = 0;
                    int bouger_colonnes = 0;
                    if (direction.ToLower() == "se") { bouger_lignes = 1; bouger_colonnes = 1; }
                    else { bouger_lignes = -1; bouger_colonnes = -1; }
                    while (ligne < matrice.GetLength(0) && ligne >= 0 && colonne < matrice.GetLength(1) && colonne >= 0 && indice_mot != mot.Length - 1)
                    {
                        if (matrice[ligne,colonne].ToLower() != Convert.ToString(mot[indice_mot])) { Console.WriteLine("Le mot ne se trouve pas içi !"); liste_lettres_temp.Clear(); return false; }
                        indice_mot++;
                        liste_lettres_temp.Add((ligne, colonne));
                        ligne += bouger_lignes;
                        colonne += bouger_colonnes;
                    }
                    if (indice_mot != mot.Length - 1) { Console.WriteLine("Le mot ne se trouve pas içi !"); return false; } /// le parcours est moins le long que le nombre de lettre -> le mot ne se trouve pas ici
                    liste_lettres_temp.Add((ligne, colonne));
                    foreach ((int, int) coords in liste_lettres_temp) { if (!(liste_lettres_vertes.Contains(coords))) { liste_lettres_vertes.Add(coords); } }
                    return true;
            }
            Console.WriteLine("Veuillez entrer une direction valide");
            return false;
        }

        public string[,] mots_a_trouver_en_colonne()
        /// Cette méthode permet de renvoyer une matrice de chaines de caractère contenant les mots à rechercher dans la grille
        /// elle permettra d'afficher les mots à trouver en colonne de même taille que le nombre de lignes de la grille afin de rendre le tout plus joli.
        {
            string[,] mat = new string[matrice.GetLength(0), matrice.GetLength(0)];
            int colonne = 0;
            int cpt = 0;
            int ligne = 0;
            while (colonne < matrice.GetLength(1))
            {
                ligne = 0;
                while (ligne < matrice.GetLength(0))
                {
                    if (cpt >= mot_recherches.Count()) { mat[ligne, colonne] = "rien"; } // si mat a été remplit avec tous les mots à rechercher, alors on arrête de remplir mat
                                                                                         // car il n'y a plus rien à afficher. Plus justement on remplit les derniers élément avec la chaine 
                                                                                         // "rien" pour éviter des problème plus tard avec l'affichage de la matrice et le fait que certains éléments soient nuls
                    else { mat[ligne, colonne] = mot_recherches[cpt]; }
                    ligne++;
                    cpt++;

                }
                colonne++;
            }
            return mat;
        }

        public void decor_noel(int indice)
        /// permet simplement de changer la couleur d'affichage des caractères en rouge ou en blanc en fonction de la parité de l'indice de boucle
        /// pour donner un effet graphique de noël.
        {
            if (indice % 2 == 0) { Console.ForegroundColor = ConsoleColor.Red; } else { Console.ForegroundColor = ConsoleColor.White; }
        }

        public void retirer_mot(string mot)
        /// permet de retirer un mot de la liste des mots à rechercher
        { 
            this.mot_recherches.Remove(mot.ToUpper());
        }

        public string affiche_mots()
        /// permet d'afficher sur une ligne la liste des mots à trouver
        {
            string chaine = "";
            foreach(string mots in this.mot_recherches) { chaine += mots + " "; }
            return chaine;
        }
        

        // Les propriétés suivantes permettent respectivement d'accéder dans le programme principal à la diffculté de la grille ainsi qu'au nombre de mots qu'il reste à trouver.
        public int Difficulte { get { return this.difficulte; } }
        public int nb_mots { get { return this.mot_recherches.Count; } }


        

        public string[,] GenerateurGrille(int ligne, int colonne, int difficulte, string langue, int nombre_mots) // On prend en paramètres la difficulté et les dimensions qui dépendent de la difficulté, ainsi que la langue.
        {
            string[] lines;
            var random = new Random(); //  On définit une variable aléatoire pour définir nos futures variables.
            if (langue == "FR")
            {
                lines = File.ReadAllLines("MotsPossiblesFR" + ".txt"); // On crée un tableau contenant les informations du fichier en français, avec chaque indice correspondant à une ligne.
            }
            else
            {
                lines = File.ReadAllLines("MotsPossiblesEN" + ".txt"); // On crée un tableau contenant les informations du fichier en anglais, avec chaque indice correspondant à une ligne.
            }
            int erreurs = 0; // On initialise le nombre d'erreurs à 0 pour pouvoir remplir la condition du while.
            string[,] grille = new string[ligne, colonne]; // On crée la grille correspondant aux mots mélés.
            string[] mots2 = lines[1].Split(' ');  // On crée un tableau pour chaque longueur de mots, comportant tous les mots de la longueur.
            List<string> strings2 = new List<string>(); // On crée une liste pour chaque longueur de mots, avec les mêmes éléments que les tableaux.

            for (int i = 0; i < mots2.Length; i++)
            {
                strings2.Add(mots2[i]);
            }

            string[] mots3 = lines[3].Split(' ');
            List<string> strings3 = new List<string>();

            for (int i = 0; i < mots3.Length; i++)
            {
                strings3.Add(mots3[i]);
            }

            string[] mots4 = lines[5].Split(' ');
            List<string> strings4 = new List<string>();

            for (int i = 0; i < mots4.Length; i++)
            {
                strings4.Add(mots4[i]);
            }
            string[] mots5 = lines[7].Split(' ');
            List<string> strings5 = new List<string>();

            for (int i = 0; i < mots5.Length; i++)
            {
                strings5.Add(mots5[i]);
            }
            string[] mots6 = lines[9].Split(' ');
            List<string> strings6 = new List<string>();

            for (int i = 0; i < mots6.Length; i++)
            {
                strings6.Add(mots6[i]);
            }
            string[] mots7 = lines[11].Split(' ');
            List<string> strings7 = new List<string>();

            for (int i = 0; i < mots7.Length; i++)
            {
                strings7.Add(mots7[i]);
            }
            string[] mots8 = lines[13].Split(' ');
            List<string> strings8 = new List<string>();

            for (int i = 0; i < mots8.Length; i++)
            {
                strings8.Add(mots8[i]);
            }
            string[] mots9 = lines[15].Split(' ');
            List<string> strings9 = new List<string>();

            for (int i = 0; i < mots9.Length; i++)
            {
                strings9.Add(mots9[i]);
            }
            string[] mots10 = lines[17].Split(' ');
            List<string> strings10 = new List<string>();

            for (int i = 0; i < mots10.Length; i++)
            {
                strings10.Add(mots10[i]);
            }
            string[] mots11 = lines[19].Split(' ');
            List<string> strings11 = new List<string>();

            for (int i = 0; i < mots11.Length; i++)
            {
                strings11.Add(mots11[i]);
            }
            string[] mots12 = lines[21].Split(' ');
            List<string> strings12 = new List<string>();

            for (int i = 0; i < mots12.Length; i++)
            {
                strings12.Add(mots12[i]);
            }
            string[] mots13 = lines[23].Split(' ');
            List<string> strings13 = new List<string>();

            for (int i = 0; i < mots13.Length; i++)
            {
                strings13.Add(mots13[i]);
            }
            string[] mots14 = lines[25].Split(' ');
            List<string> strings14 = new List<string>();

            for (int i = 0; i < mots14.Length; i++)
            {
                strings14.Add(mots14[i]);
            }
            string[] mots15 = lines[27].Split(' ');
            List<string> strings15 = new List<string>();

            for (int i = 0; i < mots15.Length; i++)
            {
                strings15.Add(mots15[i]);
            }
            List<string> mots = new List<string>(); // On crée notre liste de mots à trouver.
            while (nombre_mots > 0 && erreurs < 1000) // On estime qu'au délà de 1000 éxecutions sans rajout de mots, la grille est suffisamment remplie pour permettre de jouer.
            {
                int indiceligne = random.Next(0, ligne); // On définit aléatoirement les coordonnées de départ du mot que l'on va choisir.
                int indicecolonne = random.Next(0, colonne);
                int longueur = random.Next(2, 16); // On définit aléatoirement la longueur du mot.
                string orientation;
                string mot;

                if (difficulte == 1) // Pour chaque difficulté, on choisit l'orientation aléatoirement, correspondant aux choix disponibles.
                {
                    string[] directions = { "S", "E" };
                    orientation = directions[random.Next(directions.Length)];
                }
                else if (difficulte == 2)
                {
                    string[] directions = { "N", "S", "E", "O" };
                    orientation = directions[random.Next(directions.Length)];
                }
                else if (difficulte == 3)
                {
                    string[] directions = { "N", "S", "E", "O", "SO" };
                    orientation = directions[random.Next(directions.Length)];
                }
                else if (difficulte == 4)
                {
                    string[] directions = { "N", "S", "E", "O", "SE", "SO" };
                    orientation = directions[random.Next(directions.Length)];
                }
                else
                {
                    string[] directions = { "N", "S", "E", "O", "NO", "NE", "SO", "SE" };
                    orientation = directions[random.Next(directions.Length)];
                }

                if (longueur == 2) // Pour chaque longueur de mots, nous allons :
                {
                    if (strings2.Count() != 0) // - vérifier qu'il y a encore des éléments dans la liste, dans le cas contraire on augmente la valeur des erreurs,
                    {
                        mot = strings2[random.Next(strings2.Count())];
                        if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false) // - vérifier que le mot est plaçable dans la grille, en fonction de sa longueur et des lettres qui le composent,
                        {
                            erreurs++; // si le mot n'est pas plaçable, on augmente la valeur des erreurs,
                        }
                        else
                        {
                            poser(grille, indiceligne, indicecolonne, orientation, mot); // et si il est plaçable, on le place, on enlève le mot de la liste de longueur correspondante, pour ne pas mettre le même mot en double,
                            strings2.Remove(mot);
                            mots.Add(mot); // et on le rajoute à notre liste de mots à trouver et on réinitialise le nombre d'erreurs.
                            erreurs = 0;
                            nombre_mots--;
                        }



                    }
                    else
                    {
                        erreurs++;
                    }
                }
                if (longueur == 3)
                {
                    if (strings2.Count() != 0)
                    {
                        mot = strings3[random.Next(strings3.Count())];
                        if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false)
                        {
                            erreurs++;
                        }
                        else
                        {
                            poser(grille, indiceligne, indicecolonne, orientation, mot);
                            strings3.Remove(mot);
                            mots.Add(mot);
                            erreurs = 0;
                            nombre_mots--;

                        }
                    }
                    else
                    {
                        erreurs++;
                    }
                }
                if (longueur == 4)
                {
                    if (strings2.Count() != 0)
                    {
                        mot = strings4[random.Next(strings4.Count())];
                        if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false)
                        {
                            erreurs++;
                        }
                        else
                        {
                            poser(grille, indiceligne, indicecolonne, orientation, mot);
                            strings4.Remove(mot);
                            mots.Add(mot);
                            erreurs = 0;
                            nombre_mots--;
                        }
                    }
                    else
                    {
                        erreurs++;
                    }
                }
                if (longueur == 5)
                {
                    if (strings2.Count() != 0)
                    {
                        mot = strings5[random.Next(strings5.Count())];
                        if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false)
                        {
                            erreurs++;
                        }
                        else
                        {
                            poser(grille, indiceligne, indicecolonne, orientation, mot);
                            strings5.Remove(mot);
                            mots.Add(mot);
                            erreurs = 0;
                            nombre_mots--;
                        }
                    }
                    else
                    {
                        erreurs++;
                    }
                }
                if (longueur == 6)
                {
                    mot = strings6[random.Next(strings6.Count())];
                    if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false)
                    {
                        erreurs++;
                    }
                    else
                    {
                        poser(grille, indiceligne, indicecolonne, orientation, mot);
                        strings6.Remove(mot);
                        mots.Add(mot);
                        erreurs = 0;
                        nombre_mots--;
                    }
                }
                if (longueur == 7)
                {
                    mot = strings7[random.Next(strings7.Count())];
                    if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false)
                    {
                        erreurs++;
                    }
                    else
                    {
                        poser(grille, indiceligne, indicecolonne, orientation, mot);
                        strings7.Remove(mot);
                        mots.Add(mot);
                        erreurs = 0;
                        nombre_mots--;
                    }
                }
                if (longueur == 8)
                {
                    mot = strings8[random.Next(strings8.Count())];
                    if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false)
                    {
                        erreurs++;
                    }
                    else
                    {
                        poser(grille, indiceligne, indicecolonne, orientation, mot);
                        strings8.Remove(mot);
                        mots.Add(mot);
                        erreurs = 0;
                        nombre_mots--;
                    }
                }
                if (longueur == 9)
                {
                    mot = strings9[random.Next(strings9.Count())];
                    if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false)
                    {
                        erreurs++;
                    }
                    else
                    {
                        poser(grille, indiceligne, indicecolonne, orientation, mot);
                        strings9.Remove(mot);
                        mots.Add(mot);
                        erreurs = 0;
                        nombre_mots--;
                    }
                }
                if (longueur == 10)
                {
                    mot = strings10[random.Next(strings10.Count())];
                    if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false)
                    {
                        erreurs++;
                    }
                    else
                    {
                        poser(grille, indiceligne, indicecolonne, orientation, mot);
                        strings10.Remove(mot);
                        mots.Add(mot);
                        erreurs = 0;
                        nombre_mots--;
                    }
                }
                if (longueur == 11)
                {
                    mot = strings11[random.Next(strings11.Count())];
                    if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false)
                    {
                        erreurs++;
                    }
                    else
                    {
                        poser(grille, indiceligne, indicecolonne, orientation, mot);
                        strings11.Remove(mot);
                        mots.Add(mot);
                        erreurs = 0;
                        nombre_mots--;
                    }
                }
                if (longueur == 12)
                {
                    mot = strings12[random.Next(strings12.Count())];
                    if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false)
                    {
                        erreurs++;
                    }
                    else
                    {
                        poser(grille, indiceligne, indicecolonne, orientation, mot);
                        strings12.Remove(mot);
                        mots.Add(mot);
                        erreurs = 0;
                        nombre_mots--;
                    }
                }
                if (longueur == 13)
                {
                    mot = strings13[random.Next(strings13.Count())];
                    if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false)
                    {
                        erreurs++;
                    }
                    else
                    {
                        poser(grille, indiceligne, indicecolonne, orientation, mot);
                        strings13.Remove(mot);
                        mots.Add(mot);
                        erreurs = 0;
                        nombre_mots--;
                    }
                }
                if (longueur == 14)
                {
                    mot = strings14[random.Next(strings14.Count())];
                    if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false)
                    {
                        erreurs++;
                    }
                    else
                    {
                        poser(grille, indiceligne, indicecolonne, orientation, mot);
                        strings14.Remove(mot);
                        mots.Add(mot);
                        erreurs = 0;
                        nombre_mots--;
                    }
                }
                if (longueur == 15)
                {
                    mot = strings15[random.Next(strings15.Count())];
                    if (verifierposable(grille, indiceligne, indicecolonne, orientation, mot) == false)
                    {
                        erreurs++;
                    }
                    else
                    {
                        poser(grille, indiceligne, indicecolonne, orientation, mot);
                        strings15.Remove(mot);
                        mots.Add(mot);
                        erreurs = 0;
                        nombre_mots--;
                    }
                }
            }
            string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" }; // On crée un tableau avec toutes les lettres de l'alphabet
            for (int i = 0; i < grille.GetLength(0); i++)
            {
                for (int j = 0; j < grille.GetLength(1); j++)
                {
                    if (grille[i, j] == null)
                    {
                        grille[i, j] = alphabet[random.Next(alphabet.Length)]; // pour les ajouter aléatoirement aux cases vides de la grille de mots mélés.
                    }
                }
            }
            this.mot_recherches = mots;
            return grille;
        }

        public bool verifierposable(string[,] grille, int indiceligne, int indicecolonne, string orientation, string mot)          // Cette fonction sert à vérifier que le mot entré en paramètre peut bien être placé dans la grille en fonction de son orientation et de sa case de départ
        {
            bool verifier = true;
            if (orientation == "N")
            {
                if (mot.Length > indiceligne)
                {
                    verifier = false;                                                                                           // Pour chaque orientation on vérifie :
                }                                                                                                               // - que la longueur du mot peut rentrer dans la grille à partir des indices de la ligne et de la colonne
                else
                {                                                                                                          // - et que les lettres du mots correspondent aux lettres de la grille sur son trajet, ou si les cases sont vides 
                    int compteur2 = 0;
                    for (int i = indiceligne; i > indiceligne - mot.Length; i--)
                    {
                        if (Convert.ToString(mot[compteur2]) != grille[i, indicecolonne] && grille[i, indicecolonne] != null)
                        {
                            verifier = false;
                        }
                        compteur2++;
                    }
                }
            }
            if (orientation == "S")
            {
                if (mot.Length > grille.GetLength(0) - indiceligne)
                {
                    verifier = false;
                }
                else
                {
                    int compteur2 = 0;
                    for (int i = indiceligne; i < indiceligne + mot.Length; i++)
                    {
                        if (Convert.ToString(mot[compteur2]) != grille[i, indicecolonne] && grille[i, indicecolonne] != null)
                        {
                            verifier = false;
                        }
                        compteur2++;
                    }

                }
            }
            if (orientation == "O")
            {
                if (mot.Length > indicecolonne)
                {
                    verifier = false;
                }
                else
                {
                    int compteur2 = 0;
                    for (int i = indicecolonne; i > indicecolonne - mot.Length; i--)
                    {
                        if (Convert.ToString(mot[compteur2]) != grille[indiceligne, i] && grille[indiceligne, i] != null)
                        {
                            verifier = false;
                        }
                        compteur2++;
                    }
                }
            }
            if (orientation == "E")
            {
                if (mot.Length > grille.GetLength(1) - indicecolonne)
                {
                    verifier = false;
                }
                else
                {
                    int compteur2 = 0;
                    for (int i = indicecolonne; i < indicecolonne + mot.Length; i++)
                    {
                        if (Convert.ToString(mot[compteur2]) != grille[indiceligne, i] && grille[indiceligne, i] != null)
                        {
                            verifier = false;
                        }
                        compteur2++;
                    }
                }
            }
            if (orientation == "NO")
            {
                if (mot.Length > indicecolonne || mot.Length > indiceligne)
                {
                    verifier = false;
                }
                else
                {
                    int compteur = indicecolonne;
                    int compteur2 = 0;
                    for (int i = indiceligne; i > indiceligne - mot.Length && compteur > indicecolonne - mot.Length; i--)
                    {
                        if (Convert.ToString(mot[compteur2]) != grille[i, compteur] && grille[i, compteur] != null)
                        {
                            verifier = false;
                        }
                        compteur--;
                        compteur2++;
                    }
                }
            }
            if (orientation == "NE")
            {
                if (mot.Length > indiceligne || mot.Length > grille.GetLength(1) - indicecolonne)
                {
                    verifier = false;
                }
                else
                {
                    int compteur = indicecolonne;
                    int compteur2 = 0;
                    for (int i = indiceligne; i > indiceligne - mot.Length && compteur < indicecolonne + mot.Length; i--)
                    {
                        if (Convert.ToString(mot[compteur2]) != grille[i, compteur] && grille[i, compteur] != null)
                        {
                            verifier = false;
                        }
                        compteur++;
                        compteur2++;
                    }
                }
            }
            if (orientation == "SE")
            {
                if (mot.Length > grille.GetLength(0) - indiceligne || mot.Length > grille.GetLength(1) - indicecolonne)
                {
                    verifier = false;
                }
                else
                {
                    int compteur = indicecolonne;
                    int compteur2 = 0;
                    for (int i = indiceligne; i < indiceligne + mot.Length && compteur < indicecolonne + mot.Length; i++)
                    {
                        if (Convert.ToString(mot[compteur2]) != grille[i, compteur] && grille[i, compteur] != null)
                        {
                            verifier = false;

                        }
                        compteur++;
                        compteur2++;
                    }
                }
            }
            if (orientation == "SO")
            {
                if (mot.Length > grille.GetLength(0) - indiceligne || mot.Length > indicecolonne)
                {
                    verifier = false;
                }
                else
                {
                    int compteur = indicecolonne;
                    int compteur2 = 0;

                    for (int i = indiceligne; i < indiceligne + mot.Length && compteur > indicecolonne - mot.Length; i++)
                    {
                        if (Convert.ToString(mot[compteur2]) != grille[i, compteur] && grille[i, compteur] != null)
                        {
                            verifier = false;
                        }
                        compteur--;
                        compteur2++;
                    }
                }
            }
            return verifier;
        }
        public void poser(string[,] grille, int indiceligne, int indicecolonne, string orientation, string mot)                       // Cette fonction permet de placer un mot dans la grille en fonction de son orientation et de sa case de départ.
        {
            if (orientation == "N")
            {
                int compteur2 = 0;

                for (int i = indiceligne; i > indiceligne - mot.Length; i--)                                                        // Pour chaque orientation on place le mot.
                {

                    grille[i, indicecolonne] = Convert.ToString(mot[compteur2]);
                    compteur2++;

                }
            }
            if (orientation == "S")
            {
                int compteur2 = 0;
                for (int i = indiceligne; i < indiceligne + mot.Length; i++)
                {

                    grille[i, indicecolonne] = Convert.ToString(mot[compteur2]);
                    compteur2++;
                }
            }
            if (orientation == "O")
            {
                int compteur2 = 0;
                for (int i = indicecolonne; i > indicecolonne - mot.Length; i--)
                {

                    grille[indiceligne, i] = Convert.ToString(mot[compteur2]);
                    compteur2++;
                }
            }
            if (orientation == "E")
            {
                int compteur2 = 0;
                for (int i = indicecolonne; i < indicecolonne + mot.Length; i++)
                {

                    grille[indiceligne, i] = Convert.ToString(mot[compteur2]);
                    compteur2++;
                }
            }
            if (orientation == "NO")
            {
                int compteur2 = 0;
                int compteur = indicecolonne;
                for (int i = indiceligne; i > indiceligne - mot.Length && compteur > indicecolonne - mot.Length; i--)
                {

                    grille[i, compteur] = Convert.ToString(mot[compteur2]);

                    compteur--;
                    compteur2++;
                }
            }
            if (orientation == "NE")
            {

                int compteur = indicecolonne;
                int compteur2 = 0;
                for (int i = indiceligne; i > indiceligne - mot.Length && compteur < indicecolonne + mot.Length; i--)
                {
                    grille[i, compteur] = Convert.ToString(mot[compteur2]);

                    compteur++;
                    compteur2++;
                }
            }
            if (orientation == "SE")
            {
                int compteur2 = 0;
                int compteur = indicecolonne;
                for (int i = indiceligne; i < indiceligne + mot.Length && compteur < indicecolonne + mot.Length; i++)
                {
                    grille[i, compteur] = Convert.ToString(mot[compteur2]);

                    compteur++;
                    compteur2++;
                }
            }
            if (orientation == "SO")
            {
                int compteur2 = 0;
                int compteur = indicecolonne;
                for (int i = indiceligne; i < indiceligne + mot.Length && compteur > indicecolonne - mot.Length; i++)
                {
                    grille[i, compteur] = Convert.ToString(mot[compteur2]);

                    compteur--;
                    compteur2++;
                }
            }
        }



    }
}
