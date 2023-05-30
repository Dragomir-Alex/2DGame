using _2DGame.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Entities.Enemies
{
    public interface IEnemy
    {
        public Health Health { get; set; }
        public int AttackDamage { get; }
        public int Score { get; }

        public void OnPlayerDetection(Player player);
    }
}
