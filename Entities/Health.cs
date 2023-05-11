using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DGame.Entities
{
    public class Health
    {
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; }

        public Health(int maxHealth)
        {
            MaxHealth = (maxHealth <= 0) ? 0 : maxHealth;
            CurrentHealth = maxHealth;
        }

        public void Damage(int amount) { CurrentHealth = (CurrentHealth - amount <= 0) ? 0 : (CurrentHealth - amount); }
        public void Heal(int amount) { CurrentHealth = (CurrentHealth + amount >= MaxHealth) ? MaxHealth : (CurrentHealth + amount); }
        public void Reset() { CurrentHealth = MaxHealth; }
    }
}
