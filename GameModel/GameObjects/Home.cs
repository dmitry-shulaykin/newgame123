using System.Linq;

namespace GameModel
{
    public class Home : GameObject
    {
        private float RestTime = 1.5f;

        private float restTime = 0.0f;

        private float HealingRadius = 5.0f;
        
        public Turret Turret1 { get; set; }
        
        public Turret Turret2 { get; set; }

        public Home(Model model,Vector position, Player player)
        {
            Model = model;
            Radius = HealingRadius;
            Player = player;
            
            Acceleration = 0.0f;
            Velocity = 0.0f;
            MaxVelocity = 0.0f;

            Position = position;

            Turret1 = new Turret{Player = Player, Position = Position + new Vector(Radius, 3.0f, 0.0f)};
            
            Turret2 = new Turret{Player = Player, Position = Position + new Vector(-Radius, 3.0f, 0.0f)};
            
            Turret1.AddWeapon(Model.weaponTypes[0].GetInstance());
            Turret2.AddWeapon(Model.weaponTypes[0].GetInstance());
//            Orientation = new Vector(1.0f, 0.0f, 0.0f);

            Model.AddGameObject(Turret1);
            Model.AddGameObject(Turret2);
        }
        
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (restTime < 0.0f)
            {
                var unitsAtHome = Model.GameObjects.Where(u => u.Position.Distance(Position) < HealingRadius);

                foreach (var unit in unitsAtHome)
                {
                    unit.Heal(10.0f);
                }
                
                restTime = RestTime;
            }

            restTime -= deltaTime;
        }
    }
}