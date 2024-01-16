Le server
- Créer une machine virtuelle avec ubuntu server en utilisant VMWARE Workstation

Pour installer mysql, nous allons écrire les commande suivante:
	sudo apt-get update
	sudo apt-get install mysql-server


Ensuite, pour entrer dans mysql, nous allons écrire:
	sudo mysql -u root -p


Nous allons ensuite créer la database, nous allons taper la commandes:
	`create database nom_de_la_db_ici`


Créons maintenant un utilisateur:
	create user "utilisateur"@"%" identified by "mot_de_passe";
	exit;
