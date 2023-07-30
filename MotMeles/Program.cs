using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MotMeles
{
    class Program
    {
        static void Main(string[] args)
        {
            string langue;
            do
            {
                Console.WriteLine("Dans quelle langue voulez-vous jouer ? (FR ou EN)");
                langue = Console.ReadLine().ToUpper();
            }
            while (langue != "FR" && langue != "EN");

            Dictionnaire dico;

            if (langue == "FR") { dico = new Dictionnaire(langue, "MotsPossiblesFR"); }
            else { dico = new Dictionnaire(langue, "MotsPossiblesEN"); }
            
            
            int choix;
            bool result;
            do 
            { 
                Console.WriteLine("Voulez-vous jouer avec des grilles générées aléatoirement ou bien des grilles pré-enregistrées ? \n                          ( Aléatoires : tappez 1, Préenregistrées tappez 2)");
                string chaine_test = Console.ReadLine();
                result = int.TryParse(chaine_test, out choix);
            }
            while ( !(result) && choix != 1 && choix != 2);
            
            
       
            int nb_joueurs = dico.test_entree_entiere("Combien de joueurs êtes-vous ? (entrez un nombre entier)");



            Joueur[] liste_joueurs = new Joueur[nb_joueurs];
            Joueur joueur;
            for (int i = 0; i < nb_joueurs; i++)
            {
                Console.WriteLine("Quel votre nom joueur " + (i+1) + " ?");
                string nom = Console.ReadLine();
                joueur = new Joueur(nom, dico);
                liste_joueurs[i] = joueur;
            }


            int temps_tours = dico.test_entree_entiere("Combien de secondes voulez-vous que dure un tour ? (entrez un nombre entier)");

            Plateau[] liste_plateau;
            if (choix == 2)
            {
                string[] liste_grilles;
                if (langue == "EN") { liste_grilles = new string[] { "Difficulte1_en", "Difficulte2_en", "Difficulte3_en", "Difficulte4_en", "Difficulte5_en" }; }
                else { liste_grilles = new string[] { "Difficulte1_fr", "Difficulte2_fr", "Difficulte3_fr", "Difficulte4_fr", "Difficulte5_fr" }; }
                Plateau p1 = new Plateau(liste_grilles[0], "no", dico, 0, 0, 0, 0);
                Plateau p2 = new Plateau(liste_grilles[1], "no", dico, 0, 0, 0, 0);
                Plateau p3 = new Plateau(liste_grilles[2], "no", dico, 0, 0, 0, 0);
                Plateau p4 = new Plateau(liste_grilles[3], "no", dico, 0, 0, 0, 0);
                Plateau p5 = new Plateau(liste_grilles[4], "no", dico, 0, 0, 0, 0);
                
                liste_plateau = new Plateau[] { p1,p2,p3,p4,p5 };
            }
            else
            {
                int nb_grilles = dico.test_entree_entiere("Sur combien de grilles voulez-vous jouer ? (entrez un nombre entier)");
                liste_plateau = new Plateau[nb_grilles];
                int lignes = 7;
                int colonnes = 6;
                int difficulte;
                int nombre_mots = 8;
                var random = new Random();
                string[] liste_grilles = new string[] { "Difficulte1_en", "Difficulte2_en", "Difficulte3_en", "Difficulte4_en", "Difficulte5_en" };
                for (int i = 1; i <= nb_grilles; i++)
                {
                    
                    if (i%5 != 0) { difficulte = i % 5 ; }
                    else { difficulte = 5; }
                    Plateau pi = new Plateau("Aucun", "yes", dico, lignes, colonnes, difficulte,nombre_mots);
                    nombre_mots += random.Next(2, 7);
                    liste_plateau[i - 1] = pi;
                    pi.ToFile(liste_grilles[i - 1]);
                    lignes++;
                    colonnes++;
                }

            }

            
            
            Jeu game = new Jeu(dico, liste_plateau, liste_joueurs, temps_tours);
            
            
            Console.Write("                 ");
            for (int i = 0; i <= 20; i++) { liste_plateau[0].decor_noel(i); Console.Write("*"); }
            Console.Write("Mots-Meles");
            for (int i = 0; i <= 20; i++) { liste_plateau[0].decor_noel(i); Console.Write("*"); }
            Console.WriteLine();
            game.jouer();
            
        }

        static void Tests_Unitaires()
        /// Quelque tests unitaires pour s'assurer de la bonne fonctionnalité de certaines fonctions
        {

            // Tests de la classe Dictionnaire
            Dictionnaire dico_francais = new Dictionnaire("français", "MotsPossiblesFR");
            Dictionnaire dico_anglais = new Dictionnaire("anglais", "MotsPossiblesEN");
            string[] liste_grilles = { "CasSimple", "CasComplexe" };
            Plateau p1 = new Plateau(liste_grilles[0], "no", dico_francais, 0, 0, 0,0);
            if (dico_francais.RechDichoRecursif("avoir", dico_francais.nombre_mots_n_lettres(4))) { Console.WriteLine("Le mot avoir existe dans le dictionnaire français"); } // test du cas où le mot existe dans le dictionnaire français
            if (!(dico_francais.RechDichoRecursif("avoire", dico_francais.nombre_mots_n_lettres(5)))) { Console.WriteLine("Le mot avoire n'existe pas dans le dictionnaire français"); } // test du cas où le mot n'existe pas dans le dictionnaire français

            if (dico_anglais.RechDichoRecursif("do", dico_anglais.nombre_mots_n_lettres(2))) { Console.WriteLine("Le mot do existe dans le dictionnaire anglais"); } // test du cas où le mot existe dans le dictionnaire anglais
            if (!(dico_anglais.RechDichoRecursif("gottene", dico_anglais.nombre_mots_n_lettres(7)))) { Console.WriteLine("Le mot gottene n'existe pas dans le dictionnaire anglais"); } // test du cas où le mot n'existe pas dans le dictionnaire anglais

           

            // Tests de la classe joueur
            Joueur joueur1 = new Joueur("Yannis", dico_francais);
            Console.WriteLine(joueur1.toString());
            joueur1.Add_score(5);
            joueur1.Add_points();
            joueur1.Add_mot("mouton");
            Console.WriteLine(joueur1.toString());

            // Tests de la classe Plateau

            
            Console.WriteLine(p1.affiche_mots());
            Console.WriteLine(p1.nb_mots);
            p1.retirer_mot("Voiture");
            Console.WriteLine(p1.affiche_mots());
            Console.WriteLine(p1.nb_mots);
            p1.toString_ameliore();
            p1.Test_Plateau("voyage", 1, 2, "S");
            p1.Test_Plateau("voyage", 2, 1, "E");
            p1.toString_ameliore();
            p1.Test_Plateau("anticonstitutionnellement", 1, 2, "S"); /// test du cas où le mot a plus de 15 lettres
        }
    }
}
