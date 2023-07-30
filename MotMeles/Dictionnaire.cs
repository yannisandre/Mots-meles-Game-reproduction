using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotMeles
{
    public class Dictionnaire
    {
        private List<string[]> liste_mots;
        private string langue;

        public Dictionnaire(string langue,string nom_fichier)
        {
            this.langue = langue;
            this.liste_mots = new List<string[]>();

            string[] lines = File.ReadAllLines(nom_fichier + ".txt");
            int cpt = 0;
            /// Ici on ajoute deux éléments null à la liste car il n'existe que des mots de 2 lettres ou plus et le fait d'ajouter deux éléments inutiles permet de
            /// bien indexer la liste avec les mots de 2 lettres ayant l'indice 2, deux de n lettres l'indice n etc.
            liste_mots.Add(null);
            liste_mots.Add(null);
            foreach (string e in lines)
            {
                if (cpt % 2 == 1) { liste_mots.Add(e.Split(' ')); } /// On ajoute les mots de n lettresà la liste des mots à l'indice n, le modulo 2 permet d'ajouter les mots 
                                                                    /// seulement une fois sur deux car dans le fichier dictionnaire une ligne sur deux sert seulement à 
                                                                    /// spécifier la taille des mots de la liste à venir.
                cpt++;
            }
        }

        public bool RechDichoRecursif(string mot, int fin, int debut = 0, int milieu = 0)
        /// Cette fonction recherche de manière dichotomique récursive un mot dans le dictionnaire anglais ou français.
        {
            if ((mot.Length < 2) || (mot.Length > 15) || milieu == liste_mots[mot.Length].Length) { return false; }
            milieu += (fin - debut) / 2;
            if (debut + 1 == fin) { return false; }
            string test = liste_mots[mot.Length][milieu].ToLower();
            if (mot == test) { return true; }
            if (string.Compare(mot, test) > 0) { return RechDichoRecursif(mot, fin, milieu, milieu); }
            else { return RechDichoRecursif(mot, milieu, debut, 0); }
        }

        public int nombre_mots_n_lettres(int n)
        /// Cette fonction a pour objectif de retourner le nombre de mots de taille n que contient que dictionnaire
        {
            return liste_mots[n].Length;
        }

        public string toString()
        /// Cette fonction permet d'indiquer à l'utilisateurs combien de mots de la longueur n existe-t-il dans le dictionnaire
        {
            string chaine = "Le dictionnaire " + this.langue + " contient : ";
            for (int i = 2; i <= 15; i++)
            {
                chaine += liste_mots[i].Length + " mots de longueur " + i + "\n";
            }
            return chaine;
        }

        public int test_entree_entiere(string chaine)
        /// permet de ne pas faire planter le programme si l'utilisateur ne rentre pas un entier lorsqu'il le faut
        {
            int entier;
            bool result;
            do
            {
                Console.WriteLine(chaine);
                string chaine_test = Console.ReadLine();
                result = int.TryParse(chaine_test, out entier);
            }
            while (!(result));
            return entier;
        }

        public string Langue {  get { return this.langue; } } /// propriété permettant de récupérer à partir du programme principal la langue du dictionnaire (français ou anglais)

    }
}
