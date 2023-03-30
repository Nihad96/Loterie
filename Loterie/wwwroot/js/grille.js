document.getElementById('timer').innerHTML = "00:00";

// on désactive le bouton de validation
BoutonValider()

// heure de début (maintenant pour l'instant mais plus tard ce sera l'heure de création du tirage)
heure_debut = new Date();

// heure de fin (maintenant tirage )
// heure_fin = new Date(heure_debut.getTime() + 5 * 60000);

// fonction qui va gérer l'horloge
var  Horloge = window.setInterval(() => {
    // heure actuelle
    heure_maintenant = new Date();

    // temps restant = date_fin - date_maintenant en secondes
    nombre_secondes_restantes = Math.ceil(Math.round(Math.abs(heure_fin - heure_maintenant)) / 1000);

    // nombre de minutes et de secondes
    minutes = Math.trunc(nombre_secondes_restantes / 60);
    secondes = (nombre_secondes_restantes % 60);


    // quand il ne reste plus que deux minutes on rend le timer orange
    if (minutes == 1 && secondes == 30) {
        document.getElementById('timer').classList.add('orange');
    }

    if (minutes == 1 && secondes == 0) {
        document.getElementById('timer').classList.add('rouge');
        // on affiche le message signifiant à l'utilisateur qu'il ne peut plus participer
        document.getElementById('texte_temps_ecoule').classList.remove('d-none');
        document.getElementById('bouton_temps_ecoule').classList.remove('d-none');

        // on supprime le bouton qui valide le formulaire
        document.getElementById('grille').removeChild(document.getElementById('bouton_validation_grille'));
    }

    // on arrête la fonction si on passe à 00:00
    if (secondes == 1 && minutes == 0) {
        setTimeout(() => { document.getElementById('timer').innerHTML = '00:00'; }, 1000);
        clearInterval(Horloge);
        setTimeout(() => {
            setInterval(function () { Clignotter() }, 750);
        }, 1000)
    }

    // on ajoute un 0 afin d'homogénéiser l'affichage
    minutes = '0' + minutes;
    if (secondes < 10) {
        secondes = '0' + secondes;
    }

    // on affiche le timer
    document.getElementById('timer').innerHTML = minutes + ':' + secondes;

}, 1000); // toutes les secondes


// fonction qui permet de faire clignotter l'horloge lorsqu'on atteint 00:00
function Clignotter() {
    timer = document.getElementById('timer');
    if (timer.style.visibility == "visible") {
        timer.style.visibility = "hidden";
    }
    else {
        timer.style.visibility = "visible";
    }
}



// cette variable nous permet de compter le nombre de cellules sélectionnées
nombre_valeurs = 0;

// fonction qui permet, lorsqu'on clique sur une cellule, de la passer en sombre, et d'ajouter la valeur au dernier slot libre
function ControlButton(valeur) {

    // on effectue le script uniquement si la cellule n'est pas déjà sélectionnée et si il n'y a pas encore 6 cellules sélectionnées
    if (!document.getElementById('cellule_' + valeur).classList.contains('cellule_selectionnee') && nombre_valeurs != 6) {

        // on ajoute au bouton la classe lui permettant d'apparaitre plus sombre
        document.getElementById('cellule_' + valeur).classList.add('cellule_selectionnee');

        // on sélectionne tous les slots d'affichage pour déterminer où l'on va insérer la valeur
        var valeurs = document.getElementsByClassName("span_valeur");

        // si un slot est vide on lui assigne la valeur et on arrête de boucler
        for (var i = 0; i < valeurs.length; i++) {
            if (valeurs[i].innerHTML == "_") {
                valeurs[i].innerHTML = valeur;
                nombre_valeurs++;
                break;
            }
        }
    }

    // s'il y a 6 slots remplis on va changer l'aspect du bouton de validation et l'activer
    if (nombre_valeurs == 6) {
        document.getElementById('bouton_validation_grille').classList.remove('bouton_gris');
        document.getElementById('bouton_validation_grille').classList.add('bouton_vert');
        BoutonValider();
    }
}

// fonction permettant de réinitialiser la grille
function ReinitialiserGrille() {

    // pour chaque slot, si la valeur n'est pas un "_", on remplace la valeur par un "_"
    Array.from(document.querySelectorAll('.span_valeur')).forEach(function (el) {
        el.innerHTML = "_";
    });

    // on va maintenant déselectionner toutes les cellules
    // pour chaque cellule on retire la classe cellule_selectionnee
    Array.from(document.querySelectorAll('.cellule_selectionnee')).forEach(function (el) {
        el.classList.remove('cellule_selectionnee');
    });

    // on réaffiche le bouton gris
    document.getElementById('bouton_validation_grille').classList.add('bouton_gris');
    document.getElementById('bouton_validation_grille').classList.remove('bouton_vert');

    // enfin on repasse le nombre de valeurs sélectionnées à 0
    nombre_valeurs = 0;

    // et on désactive le bouton valider
    BoutonValider()
}



// une fonction qui gère la validation du formulaire (controle le nombre de slots encore une fois, puis effectue le post)

function Participer() {
    // il faut checker si :
    // les valeurs sont des nombres
    // si les nombres sont compris entre 1 et 49



    // on sélectionne tous les slots d'affichage
    var valeurs = document.getElementsByClassName("span_valeur");


    for (var i = 0; i < valeurs.length; i++) {

        // si la valeur du slot est égale à _ ou alors si la valeur du slot n'est pas un nombre alors on bloque
       if (!isNumber(valeurs[i].innerHTML)) {
            // alors on affiche un message d'erreur
           break;
        }

        // si la valeur du slot est un nombre, mais qu'il n'est pas compris entre 1 et 49 (modification dans l'inspecteur)
        if (parseInt(valeurs[i].innerHTML) < 1 || parseInt(valeurs[i].innerHTML) > 49) {
            alert('Petit filou. La grille va de 1 à 49 !');
            break;
        }
    }


    // si on est ici c'est que tout est bon
    // maintenant on va remplir un champ caché avec une concaténation de tous les slots pour les insérer dans la base de données
    // on va créer une variable valeur finale qui va contenir toutes les valeurs séparées d'un ';'
    var valeur_finale = "";

    // on récupère toutes les valeurs
    var span_valeur = document.getElementsByClassName('span_valeur');

    // puis on boucle sur le tableau obtenu
    for (var i = 0; i < span_valeur.length; i++) {

        // on ajoute la valeur
        valeur_finale += span_valeur[i].innerHTML;

        // puis on ajoute un point virgule sauf à la dernière itération
        if (i != span_valeur.length - 1) {
            valeur_finale += ";";
        }
    }

    // maintenant on va passer la valeur à l'input caché
    document.getElementById('valeur_participation').value = valeur_finale;

    // puis on valide le formulaire
    document.getElementById('form_grille').submit();
}


// fonction permettant de checker si n est un nombre
function isNumber(n) {
    return /^\d+$/.test(n);
}

// fonction permettant d'activer / désactiver le bouton Valider
function BoutonValider() {
    var bouton = document.getElementById("bouton_validation_grille");

    if (bouton.disabled) {
        bouton.disabled = false;
    }
    else {
        bouton.disabled = true;
    }
}