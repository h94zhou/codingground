using System.IO;
using System;
//things to add: spells maybe in char class, healing, fix scaling & enemy difficulty, add locations, intro, enemy attack when player misses
class Unit{
    protected int health;
    protected int maxHealth;
    protected int attack;
    protected int accuracy;
    protected Random rand = new Random();
    public Unit(int _health, int _attack){
        health = _health;
        maxHealth = _health;
        attack = _attack;
        accuracy = 95;
    }
    public int GetHealth(){
        return health;
    }
    public void Attack(Unit enemy){
        if (rand.Next(0, 101) <= accuracy){
            //Console.WriteLine("The attack hits for " + attack + " damage!");
            enemy.health -= attack;
        }else{
            Console.WriteLine("The attack missed!");
        }
        if (enemy.health <= 0){
            enemy.Defeated();
            //gameover
        }
    }public void Attack(Unit enemy, int damage){//special
        enemy.health -= damage;
        if (enemy.health <= 0){
            //gameover
        }
    }public void Defeated(){
        Console.WriteLine("u ded");
    }
}

class Enemy: Unit{
    static string[] type = {"slime", "goblin", "orc", "spoder", "doggo"};
    static int[] specials = {0, 5, 20, 10, 16};
    static string[] description = {"A simple green slime. Fairly harmless",
                            "Long nosed goblin. Carries a machete",
                            "Large orc, not very smart but has a lot of health",
                            "Eek a spoder! Kill it before it lays eggs",
                            "BORF BORF BORF BORK arf arf... woof"};
    static int[] exp = {5, 10, 30, 15, 50};
    private int ID;
    private bool basic;
    Mook Mooks = new Mook(type, specials, description);
    public Enemy(int _health, int _attack, int _ID) : base(_health, _attack){
        ID = _ID;
    }
    public string ShowType(){
        return Mooks.type[ID];
    }
    public void Examine(){
        Console.WriteLine(Mooks.description[ID]);
    }
    public void TakeDamage(int damage, Player player){
        health -= damage;
        if (health <= 0){
            Console.WriteLine("The " + Mooks.type[ID] + " has been defeated");
            Console.WriteLine("You gain " + exp[ID] + " exp");
            player.GainExp(exp[ID]);
        }else{
            basic = false;
            switch(Mooks.type[ID]){
                case "slime":
                    if (rand.Next(0, 101) >= 75){
                        Console.WriteLine("The slime bounces around");
                    }else if (rand.Next(0,101) >= 85){
                        Console.WriteLine("Slime charges up... and deflates");
                        Attack(player, Mooks.specials[ID]);
                    }else{
                        basic = true;
                    }
                    break;
                case "goblin":
                    if (rand.Next(0, 101) >= 85){
                        Console.WriteLine("The goblin swings its machete wildly, hitting nothing");
                    }else if (rand.Next(0,101) >= 85){
                        Console.WriteLine("The goblin swings extra hard at you. You take {0} damage", Mooks.specials[ID]);
                        Attack(player, Mooks.specials[ID]);
                    }else{
                        basic = true;
                    }
                    break;
                case "orc":
                    if (rand.Next(0, 101) >= 70){
                        Console.WriteLine("The orc scratches its head");
                    }else if (rand.Next(0,101) >= 80){
                        Console.WriteLine("The orc becomes enranged and swings its club with both hands. You take {0} damage", Mooks.specials[ID]);
                        Attack(player, Mooks.specials[ID]);
                    }else{
                        basic = true;
                    }
                    break;
                case "spoder":
                    if (rand.Next(0, 101) >= 90){
                        Console.WriteLine("Spoder lunges. Ahh get away! You take {0} damage", Mooks.specials[ID]);
                        Attack(player, Mooks.specials[ID]);
                    }else{
                        basic = true;
                    }
                    break;
                case "doggo":
                    if (rand.Next(0, 101) >= 90){
                        Console.WriteLine("Doggo sees its own tail wagging and chases it");
                    }else if (rand.Next(0,101) >= 75){
                        Console.WriteLine("The doggo didn't get enough pats");
                        Attack(player, Mooks.specials[ID]);
                    }else{
                        basic = true;
                    }
                    break;
                default:
                    break;
            }
            if (basic){
                Console.WriteLine("{0} attacks! You take {1} damage", Mooks.type[ID], attack);
                Attack(player);
            }
        }
    }
    
}

class Player: Unit{
    private int exp;
    private int level;
    private int magic;

    public Player(int _health, int _attack, int _magic) : base(_health, _attack){
        exp = 0;
        level = 1;
        magic = _magic;
    }
    public void CastSpell(int spellType, Enemy enemy){
        switch(spellType){
            case 0:
                Console.WriteLine("flameeeeeesssss");
                enemy.TakeDamage(level*5 + 5, this);
                break;
        }
    }
    public void Attack(Enemy enemy){
        Console.WriteLine("You attack the " + enemy.ShowType());
        if (rand.Next(0, 101) <= accuracy){
            //Console.WriteLine("The attack hits for " + attack + " damage!");
            enemy.TakeDamage(attack, this);
        }else{
            Console.WriteLine("The attack missed!");
        }
    }
    public void LevelUp(){
        Console.WriteLine("Your attack increased by 5! Your health increased by 10!");
        attack += 5;
        maxHealth += 10;
        health = maxHealth;
        if (level%4 == 0){
            Console.WriteLine("You've learned a new spell! wip");
        }
    }
    public void GainExp(int _exp){
        exp += _exp;
        if (exp >= level*10 + 10){
            exp -= (level*10 + 10);
            level++;
            Console.WriteLine("Level Up! You are now level " + level);
            this.LevelUp();
        }
    }
    public void ShowExp(){
        Console.WriteLine("Level: " + level);
        Console.WriteLine("EXP: " + exp);
    }
    public void Status(){
        Console.WriteLine("Health: {0}/{1}", health, maxHealth);
        Console.WriteLine("Attack: {0}", attack);
    }
}

public struct Mook{
    public string[] type;
    public int[] specials;
    public string[] description;
    public Mook(string[] _type, int[] _specials, string[] _description){
        type = _type;
        specials = _specials;
        description = _description;
    }
};

class Program
{
    enum state {idle, battle, quitting};
    enum spell {flame, analyze, heal, instagib};
    enum MookID {slime, goblin, orc, spoder, doggo};
    static void Main(){
        bool game = true;
        string action;
        state currentState = state.idle;
        Random r = new Random();
        MookID id;
        RNG rng = new RNG();
        
        Enemy enemy = new Enemy(20, 2, (int)MookID.slime);
        Player player = new Player(30, 10, 1);
        //enemy.Examine();
        //player.Attack(enemy);
        //Console.WriteLine(enemy.ShowType() + " health: " + enemy.ShowHealth());
        //player.Attack(enemy);
        player.ShowExp();

        while(game){
            switch(currentState){
                case state.idle:
                    Console.WriteLine("You stand in a field, taking in the breeze");
                    action = Console.ReadLine();
                    if (action == "fight"){
                        //create new random enemy <- random number variable(fix enemy parameters to only use ID and struct info)
                        id = (MookID)r.Next(0,5);
                        enemy = new Enemy (rng.RollStats((int)id, false), rng.RollStats((int)id, true), (int)id);
                        Console.Write("You encounter a {0}! ", id);
                        currentState = state.battle;
                    }else if (action == "quit"){
                        currentState = state.quitting;
                    }else if (action == "help" || action == "?"){
                        Console.WriteLine("fight/quit");
                    }else if (action == "exp"){
                        player.ShowExp();
                    }else if (action == "status"){
                        player.Status();
                    }else{
                        Console.WriteLine("That is not a valid action");
                    }
                    break;
                case state.battle:
                    if (enemy.GetHealth() <= 0){
                        currentState = state.idle;
                    }else{
                        Console.WriteLine("What will you do?");
                        action = Console.ReadLine();
                        if (action == "attack"){
                            player.Attack(enemy);
                        }else if (action == "examine"){
                            enemy.Examine();
                        }else if (action == "run"){
                            Console.WriteLine("You escaped");
                            currentState = state.idle;
                        }else if (action == "status" || action == "check status"){
                            player.Status();
                        }else if (action == "help" || action == "?"){
                            Console.WriteLine("attack/examine/run/status");
                        }else{
                            Console.WriteLine("Invalid action");
                        }
                    }
                    break;
                case state.quitting:
                default:
                    game = false;
                    break;
            }
        }
    }
}
class RNG{
    public int RollStats(int id, bool isAttack){     //rolls attack and health, exp scales off sum of both?
        Random rand = new Random();
        switch (id){
            case 0:
                if (isAttack){//attack
                    return (2 + rand.Next(0,4));
                }else{//health
                    return (10 + rand.Next(0,21));
                }
            case 1:
                if (isAttack){//attack
                    return (5 + rand.Next(0,6));
                }else{//health
                    return (15 + rand.Next(0,11));
                }
            case 2:
                if (isAttack){//attack
                    return (10 + rand.Next(0,11));
                }else{//health
                    return (40 + rand.Next(0,21));
                }
            case 3:
                if (isAttack){//attack
                    return (15 + rand.Next(0,6));
                }else{//health
                    return (15 + rand.Next(0,16));
                }
            case 4:
                if (isAttack){//attack
                    return (6 + rand.Next(0,9));
                }else{//health
                    return (42);
                }
            default:
                return 10;
        }
    }
}

