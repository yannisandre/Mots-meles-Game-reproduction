using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotMeles
{
    public class Jeu
    {
        private Dictionnaire dico;
        private Plateau[] liste_plateaux;
        private Plateau plateau_courant;
        private Joueur[] liste_joueurs;
        private Joueur joueur_courant;
        private DateTime chrono1;
        private DateTime chrono2;
        private DateTime chrono_final;
        private int indice_plateau;
        private int temps_total;
        private int temps_tours;

        public Jeu(Dictionnaire dico, Plateau[] liste_plateaux, Joueur[] joueurs, int temps_tours)
        /*Constructeur de la classe Jeu, dico correspond au dictionnaire (anglais ou français) choisit par le joueur, liste_plateaux est la liste des grilles de mot-meles générées
        ou bien récupérées par fichier csv. chrono et chrono2 servent respectivement à définir un laps de temps limite pour jouer au premier joueur, puis au deuxième puis au troisième etc
        jusqu'à revenir au premier. chrono_final permet à la partie de s'arrêter après un certain temps. */
        {
            this.dico = dico;
            this.liste_plateaux = liste_plateaux;
            this.indice_plateau = 0;
            this.plateau_courant = liste_plateaux[indice_plateau];
            this.liste_joueurs = joueurs;
            this.joueur_courant = joueurs[0];
            this.chrono1 = DateTime.Now;
            this.chrono2 = DateTime.Now;
            this.chrono_final = DateTime.Now;
            this.temps_tours = temps_tours; /// temps aloué au joueur pour résoudre une grille
            this.temps_total = temps_tours*liste_plateaux.Length + 30; /// temps total au bout duquel la partie s'arrête (temps d'un tour multiplié par le nombre de grilles
                                                                       /// + 30 secondes afin de laisser une marge aux joueurs lents
        }

        public void affiche_info()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("                A vous " + joueur_courant.Nom + " vous êtes sur une grille de difficulté " + plateau_courant.Difficulte);
            Console.WriteLine("                                ( " + plateau_courant.nb_mots + " mot(s) caché(s) )");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void jouer()
        {
            int i = 0;
            TimeSpan fin = DateTime.Now - chrono_final;
            TimeSpan tours1;
            TimeSpan tours2;
 
            affiche_info();
            while (fin.TotalMinutes < temps_total)
            {
                fin = DateTime.Now - chrono_final;
                chrono1 = DateTime.Now;
                chrono2 = DateTime.Now;
                tours1 = DateTime.Now - chrono2;
                tours2 = DateTime.Now - chrono2;
                while (tours1.TotalSeconds < temps_tours)
                {

                    plateau_courant.toString_ameliore();
                    Console.WriteLine("Mot  ? :");
                    string mot = Console.ReadLine();
                    int ligne = dico.test_entree_entiere("Ligne ? : ");
                    int colonne = dico.test_entree_entiere("Colonne ? : ");
                    Console.WriteLine("Direction (N;S;E;O;NE;NO;SE;SO) ? : ");
                    string direction = Console.ReadLine();
                    tours1 = DateTime.Now - chrono1;
                    if (tours1.TotalSeconds > temps_tours) { Console.WriteLine("Temps dépassé"); break; }
                    if (plateau_courant.Test_Plateau(mot, ligne, colonne, direction))
                    {
                        joueur_courant.Add_mot(mot);
                        joueur_courant.Add_score(mot.Length);
                        plateau_courant.retirer_mot(mot);
                        Console.WriteLine("Bien joué vous avez trouvé le mot " + mot + " !");
                        if (plateau_courant.nb_mots == 0)
                        {
                            if (indice_plateau == liste_plateaux.Length - 1) /// si on a finit tous les plateaux le meilleur des deux joueurs remporte la partie
                            {
                                gagner();
                                return;
                            }
                            indice_plateau++;
                            joueur_courant.Add_points();
                            Console.WriteLine(joueur_courant.toString());
                            plateau_courant = liste_plateaux[indice_plateau];
                            joueur_courant.reset_mots_trouve();
                            chrono2 = DateTime.Now;
                            if (i == liste_joueurs.Length - 1) { i = 0; } else { i++; }
                            joueur_courant = liste_joueurs[i];
                            affiche_info();

                        }
                        else
                        {
                            Console.WriteLine(joueur_courant.toString());
                           
                        }

                    }
                }
              
                if (tours1.TotalSeconds > temps_tours)
                {
                    if (indice_plateau == liste_plateaux.Length - 1) /// si on a finit tous les plateaux le meilleur des deux joueurs remporte la partie
                    {
       
                        gagner();
                        return;
                    }
                    indice_plateau++;
                    plateau_courant = liste_plateaux[indice_plateau];
                    joueur_courant.reset_mots_trouve();
                    chrono2 = DateTime.Now;
                    if (i == liste_joueurs.Length - 1) { i = 0; } else { i++; }
                    joueur_courant = liste_joueurs[i];
                    affiche_info();

                }
                while (tours2.TotalSeconds < temps_tours)
                {

                    plateau_courant.toString_ameliore();
                    Console.WriteLine("Mot  ? :");
                    string mot = Console.ReadLine();
                    int ligne = dico.test_entree_entiere("Ligne ? : ");
                    int colonne = dico.test_entree_entiere("Colonne ? : ");
                    Console.WriteLine("Direction (N;S;E;O;NE;NO;SE;SO) ? : ");
                    string direction = Console.ReadLine();
                    tours2 = DateTime.Now - chrono2;
                    if (tours2.TotalSeconds > temps_tours) { Console.WriteLine("Temps dépassé"); break; }
                    if (plateau_courant.Test_Plateau(mot, ligne, colonne, direction))
                    {
                        joueur_courant.Add_mot(mot);
                        joueur_courant.Add_score(mot.Length);
                        plateau_courant.retirer_mot(mot);
                        Console.WriteLine("Bien joué vous avez trouvé le mot " + mot + " !");
                        if (plateau_courant.nb_mots == 0)
                        {
                            
                            if (indice_plateau == liste_plateaux.Length - 1) /// si on a finit tous les plateaux le meilleur des deux joueurs remporte la partie
                            {
                                gagner();
                                return;
                            }
                            indice_plateau++;
                            joueur_courant.Add_points();
                            Console.WriteLine(joueur_courant.toString());
                            plateau_courant = liste_plateaux[indice_plateau];
                            joueur_courant.reset_mots_trouve();
                            chrono1 = DateTime.Now;
                            if (i == liste_joueurs.Length - 1) { i = 0; } else { i++; }
                            joueur_courant = liste_joueurs[i];
                            affiche_info();

                        }
                        else
                        {
                            Console.WriteLine(joueur_courant.toString());
                            
                        }
                    }
                }
                
                if (tours2.TotalSeconds > temps_tours)
                {
                    if (indice_plateau == liste_plateaux.Length - 1) /// si on a finit tous les plateaux le meilleur des deux joueurs remporte la partie
                    {
                        gagner();
                        return;
                    }
                    indice_plateau++;
                    plateau_courant = liste_plateaux[indice_plateau];
                    joueur_courant.reset_mots_trouve();
                    chrono1 = DateTime.Now;
                    if (i == liste_joueurs.Length - 1) { i = 0; } else { i++; }
                    joueur_courant = liste_joueurs[i];
                    affiche_info();

                }




            }
            Console.WriteLine("Temps de la partie écoulé !");
            gagner();
            return;

        }

        public void gagner()
        /// Lorsque la partie est finie, il faut désigner un gagnant, celui qui a compléter le plus de grilles (points du joueurs) et s'ils ont le même nombre de
        /// points alors c'est celui qui aura le plus de score (somme des tailles des mots trouvés)
        {
            Joueur gagnant = liste_joueurs[0];
            for (int k = 1; k < liste_joueurs.Length; k++)
            {
                if (liste_joueurs[k].Points > gagnant.Points) { gagnant = liste_joueurs[k]; }
                else if (liste_joueurs[k].Points == gagnant.Points)
                {
                    if (liste_joueurs[k].Score > gagnant.Score) { gagnant = liste_joueurs[k]; }
                    else if (liste_joueurs[k].Score == gagnant.Score)
                    {
                        Console.WriteLine("Egalité");
                        return;
                    }
                }
            }
            Console.WriteLine("Le joueur " + gagnant.Nom + " a gagné avec ces statistiques : " + "\n" + gagnant.toString());
        }


    }
}
