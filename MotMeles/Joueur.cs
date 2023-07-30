using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotMeles
{
    public class Joueur
    {
        private string nom;
        private List<string> mots_trouves_actuels;
        private int nb_mots_tr;
        private int points;
        private int score;
        private Dictionnaire dictionnaire;

        public Joueur(string nom, Dictionnaire dictionnaire)
        /*Constructeur de la classe joueur avec un attribut désignant son nom, la liste des mots qu'il a trouvé sur la grille actuelle, le nombre de grille qu'il a finit (points)
        , le nombre de mots qu'il a trouvé au cours de la partie (nb_mots_tr), son score qui correspond à la somme de la taille des mots qu'il a trouvé au cours de la partie ainsi
        que le dictionnaire français ou anglais en fonction du choix du joueur
        */
        {
            this.nom = nom;
            this.mots_trouves_actuels = new List<string>();
            this.points = 0;
            this.nb_mots_tr = 0;
            this.score = 0;
            this.dictionnaire = dictionnaire;
        }

        public void Add_mot(string mot)
        /// Cette fonction ajoute le mot passé en paramètre à la liste des mots trouvés par le joueur
        {
            mots_trouves_actuels.Add(mot);
        }

        public void Add_score(int val)
        /// Cette fonction incrémente le score du joueur courant de val points
        {
            score += val;
            nb_mots_tr++;
        }

        public void Add_points()
        /// Lorsque le joueur finit une grille il gagne un point
        {
            points++;
        }

        public void reset_mots_trouve()
        /// Cette fonction permet de réinitialiser les mots découverts par le joueur lorsqu'il joue sur une nouvelle grille
        {
            this.mots_trouves_actuels.Clear();
        }

        public string toString()
        /// Cette fonction a pour objectif de décrire un joueur en donnant le nombre de mots qu'il a trouvé et son score actuel
        {
            return nom + " a trouvé " + nb_mots_tr + " mot(s) et son score est de " + score + ", il a également " + points + " point(s)";
        }

        /// Propriétés permettant de récupérer respectivement les attributs privés nom,points et score du joueur à partir du programme principal
        public string Nom { get { return this.nom; } }
        public int Points { get { return this.points; } }
        public int Score { get { return this.score; } }

        








    }
}
