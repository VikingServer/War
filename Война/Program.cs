class Program
{
    public static void Main(string[] args)
    {
        Platoon red = new Platoon();
        Platoon white = new Platoon();

        Arena arena = new Arena(red, white);

        arena.Fight();
    }
}

static class Utils
{
    private static Random s_random = new Random();

    public static int GetRandomNumber(int minValue, int maxValue)
    {
        return s_random.Next(minValue, maxValue);
    }
}

interface IUsingAbility
{
    void UseAbility();
}

abstract class Fighter
{
    protected int Armor;

    public Fighter(string name, double health, double damage, int armor)
    {
        Name = name;
        Health = health;
        Damage = damage;
        Armor = armor;
    }

    public string Name { get; protected set; }
    public double Health { get; protected set; }
    public double Damage { get; protected set; }

    public virtual void ShowStats()
    {
        Console.WriteLine($"{Name}, здоровье: {Math.Round(Health, 1)}, наносимый урон: {Damage}, броня: {Armor}");
    }

    public void ShowCurrentHealth()
    {
        double health = Math.Round(Health, 3);

        Console.WriteLine($"\n{Name} здоровье: {health}");
    }

    protected virtual void TakeDamage(double damage)
    {
        double percent = 100;

        Health -= damage - (damage / percent) * Armor;
    }

    public virtual void Attack(Platoon enemies)
    {
        if (enemies.TryChooseFighter(out Fighter enemy))
        {
            enemy.TakeDamage(Damage);
        }
    }

    public bool IsDied()
    {
        return Health <= 0;
    }
}

class Knight : Fighter, IUsingAbility
{
    public Knight(string name = "Рыцарь") : base(name, 0, 0, 0)
    {
        int minHealth = 1500;
        int maxHealth = 2000;

        int minDamage = 200;
        int maxDamage = 250;

        int minArmor = 50;
        int maxArmor = 80;

        Health = Utils.GetRandomNumber(minHealth, maxHealth + 1);
        Damage = Utils.GetRandomNumber(minDamage, maxDamage + 1);
        Armor = Utils.GetRandomNumber(minArmor, maxArmor + 1);
    }

    protected override void TakeDamage(double damage)
    {
        base.TakeDamage(damage);
    }

    public void UseAbility()
    {
        DoCriticalAttack();
    }

    public override void Attack(Platoon enemies)
    {
        UseAbility();

        base.Attack(enemies);
    }

    private void DoCriticalAttack()
    {
        int percentCriticalStrike;
        int percent = 100;

        if (TryDoCriticalAttack(out percentCriticalStrike))
        {
            Damage = Damage + Damage / percent * percentCriticalStrike;
        }
    }

    private bool TryDoCriticalAttack(out int percentCriticalStrike)
    {
        int minPercent = 0;
        int maxPercent = 100;

        int minPercentCriticalStrike = 5;
        int maxPercentCriticalStrike = 40;

        percentCriticalStrike = Utils.GetRandomNumber(minPercentCriticalStrike, maxPercentCriticalStrike + 1);

        return percentCriticalStrike > Utils.GetRandomNumber(minPercent, maxPercent + 1);
    }
}

class Goblin : Fighter
{
    public Goblin(string name = "Гоблин") : base(name, 0, 0, 0)
    {
        int minHealth = 1000;
        int maxHealth = 1800;

        int minDamage = 150;
        int maxDamage = 300;

        int minArmor = 20;
        int maxArmor = 60;

        Health = Utils.GetRandomNumber(minHealth, maxHealth + 1);
        Damage = Utils.GetRandomNumber(minDamage, maxDamage + 1);
        Armor = Utils.GetRandomNumber(minArmor, maxArmor + 1);
    }
}

class MultipleAttacker : Fighter
{
    public MultipleAttacker(string name) : base(name, 0, 0, 0) { }

    public int CountMultipleAttack { get; protected set; }

    private bool CanMultipleAttack(int countEnemy)
    {
        while (CountMultipleAttack > countEnemy)
        {
            CountMultipleAttack--;

            if (CountMultipleAttack == 0)
                return false;
        }

        return true;
    }

    public override void Attack(Platoon enemies)
    {
        if (CanMultipleAttack(enemies.CountFighters))
        {
            for (int i = 0; i < CountMultipleAttack; i++)
            {
                base.Attack(enemies);
            }
        }
    }
}

class Gnome : MultipleAttacker
{
    public Gnome(string name = "Гном") : base(name)
    {
        int minHealth = 1800;
        int maxHealth = 2000;

        int minDamage = 100;
        int maxDamage = 170;

        int minArmor = 80;
        int maxArmor = 100;

        Health = Utils.GetRandomNumber(minHealth, maxHealth + 1);
        Damage = Utils.GetRandomNumber(minDamage, maxDamage + 1);
        Armor = Utils.GetRandomNumber(minArmor, maxArmor + 1);
        CountMultipleAttack = 3;
    }
}

class ElfArcher : MultipleAttacker
{
    public ElfArcher(string name = "Эльф") : base(name)
    {
        int minHealth = 1500;
        int maxHealth = 1900;

        int minDamage = 250;
        int maxDamage = 300;

        int minArmor = 50;
        int maxArmor = 70;

        int minMultipleAttack = 2;
        int maxMultipleAttack = 5;

        Health = Utils.GetRandomNumber(minHealth, maxHealth + 1);
        Damage = Utils.GetRandomNumber(minDamage, maxDamage + 1);
        Armor = Utils.GetRandomNumber(minArmor, maxArmor + 1);
        CountMultipleAttack = Utils.GetRandomNumber(minMultipleAttack, maxMultipleAttack + 1);
    }
}

class Platoon
{
    private List<Fighter> _fighters = new List<Fighter>();

    public Platoon()
    {
        int minLinesFighters = 1;
        int maxLinesFighters = 5;

        int countLinesFighters = Utils.GetRandomNumber(minLinesFighters, maxLinesFighters + 1);

        for (int i = 0; i < countLinesFighters; i++)
        {
            _fighters.Add(new Knight());
            _fighters.Add(new Gnome());
            _fighters.Add(new Goblin());
            _fighters.Add(new ElfArcher());
        }
    }

    public int CountFighters
    {
        get
        {
            return _fighters.Count;
        }
        private set { }
    }

    public bool CanFight
    {
        get
        {
            return _fighters.Count > 0;
        }
        private set { }
    }

    public void ShowInformation()
    {
        foreach (Fighter fighter in _fighters)
        {
            fighter.ShowStats();
        }
    }

    private void DeleteDiedFighters()
    {
        for (int i = 0; i < _fighters.Count; i++)
        {
            if (_fighters[i].IsDied())
                _fighters.Remove(_fighters[i]);
        }
    }

    public void Attack(Platoon enemyPlatoon)
    {
        if (CanFight != false)
        {
            if (TryChooseFighter(out Fighter attacker))
                attacker.Attack(enemyPlatoon);

            enemyPlatoon.DeleteDiedFighters();
        }
    }

    public bool TryChooseFighter(out Fighter fighter)
    {
        int indexEnemies = GetIndexEnemy(_fighters.Count);

        fighter = _fighters[indexEnemies];

        return fighter != null;
    }

    private int GetIndexEnemy(int countEnemies)
    {
        int minIndex = 0;
        int maxIndex = countEnemies;

        return Utils.GetRandomNumber(minIndex, maxIndex);
    }
}

class Arena
{
    private Platoon _platoon1;
    private Platoon _platoon2;

    public Arena(Platoon platoonRea, Platoon platoonWhite)
    {
        _platoon1 = platoonRea;
        _platoon2 = platoonWhite;
    }

    public void Fight()
    {
        ShowPlatoons();

        while (_platoon2.CanFight && _platoon1.CanFight)
        {
            _platoon1.Attack(_platoon2);
            _platoon2.Attack(_platoon1);
        }

        ShowResult();
    }

    private void ShowPlatoons()
    {
        Console.WriteLine("\nВзвод красных: ");
        _platoon1.ShowInformation();

        Console.WriteLine("\nВзвод белых: ");
        _platoon2.ShowInformation();
    }

    private void ShowResult()
    {
        if (_platoon1.CanFight == false)
        {
            Console.WriteLine("\nПобеда отряда белых");
            _platoon2.ShowInformation();
        }
        else if (_platoon2.CanFight == false)
        {
            Console.WriteLine("\nПобеда отряда красных");
            _platoon1.ShowInformation();
        }
        else
        {
            Console.WriteLine("\nОба отряда разбиты");
        }
    }
}