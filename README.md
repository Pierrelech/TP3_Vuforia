# Strict Fighter - Augmented Reality Experience 🥊

**Strict Fighter** est une application de réalité augmentée interactive développée sous **Unity 3D** utilisant le moteur de recalage **Vuforia**[^1]. Le projet met en œuvre un système de combat au tour par tour géré par la détection et l'interaction dynamique entre plusieurs marqueurs physiques[^2].

## 📺 Démo

[![Demo video](https://drive.usercontent.google.com/download?id=1_Umg3BAXSWdBk6O14AVTkwvCx566mTZL)](https://www.youtube.com/watch?v=yCMsKed7RI0)

## 🚀 Fonctionnalités Clés

* **Interaction Multi-Marqueurs :** Détection simultanée de plusieurs modèles 3D animés avec gestion dynamique de la rotation face-à-face basée sur la distance entre les marqueurs.
* **Système de Combat Évolutif :**
    * **Attaques (Type 2) :** Déclenchement d'attaques aléatoires. Les attaques évoluent sur 3 niveaux (Coup de poing → Coup de pied → Coup circulaire) avec animations et SFX dédiés[^3].
    * **Soin (Type 2) :** Utilisation d'un marqueur "Oxygen" pour restaurer la santé.
* **Mécanique d'Évolution (Type 3) :** Transformation du personnage via un marqueur spécial (Cyberpunk), modifiant le modèle 3D, les animations et les statistiques.
* **Game Logic & Gamification :** Gestion du tour par tour avec timer, système de manches et interface utilisateur (UI) dynamique.

## 🛠️ Stack Technique

* **Engine :** Unity 3D [^2]
* **AR SDK :** Vuforia Engine (Image Targets, Recalage 3D) [^1]
* **Langage :** C# (Scripts de logique de combat, calculs de distance, gestion d'états)
* **Modélisation :** Assets 3D intégrés et gestion des transitions via l'Animator Unity.

## 🎮 Implémentation du Gameplay

L'expérience repose sur trois types de marqueurs interagissant en temps réel :

* **Combattants :** S'activent à la détection et passent en mode combat par calcul de proximité.
* **Actions :** Cartes d'attaque et de soin limitées à une action par tour pour l'équilibre du jeu.
* **Évolution :** Permet de sacrifier un tour pour obtenir un modèle plus puissant.

---

## 📚 Références & Documentation
[^1]: [Vuforia Engine Documentation](https://library.vuforia.com/) - Moteur utilisé pour le tracking d'images et le recalage 3D.
[^2]: [Unity Scripting API](https://docs.unity3d.com/ScriptReference/) - Framework utilisé pour le développement de la logique de jeu en C#.
[^3]: Les assets sonores et animations proviennent de banques libres de droits adaptées pour le projet.
