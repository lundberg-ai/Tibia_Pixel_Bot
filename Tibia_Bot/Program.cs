using System.Drawing;
using System.Media;
using WindowsInput;
using WindowsInput.Native;

namespace Tibia_Bot
{
	class Program
	{
		// Coordinates and RGB values
		static Point healthPotPos = new Point(520, 67);   // Health potion check
		static Point healthSpellPos = new Point(667, 67); // Health spell check
		static Point ringSlotPos = new Point(1430, 203);  // Ring slot check
		static Point manaFullPos = new Point(936, 66);    // Full mana bar check
		static Point eatFoodPos = new Point(1426, 230);   // Eat food check
		static Point manaPotionPos = new Point(1149, 67); // Mana potion check

		// Input flags
		static bool useHealthPotions = true;
		static bool useManaPotions = true;
		static bool useHealingSpells = true;
		static bool refillRing = true;
		static bool useFood = true;

		// Input simulator for key presses
		static InputSimulator inputSimulator = new InputSimulator();

		static void Main(string[] args)
		{
			Console.WriteLine("Tibia bot started!");

			while (true)
			{
				// Log pixel colors for debugging during development
				LogPixelColor("Health Bar", healthPotPos);
				LogPixelColor("Mana Bar", manaFullPos);
				LogPixelColor("Ring Slot", ringSlotPos);
				LogPixelColor("Hunger Icon", eatFoodPos);

				// Main loop to keep checking conditions and performing actions
				if (useHealthPotions)
					CheckHealthAndUsePotion();

				if (useHealingSpells)
					CheckHealthAndUseSpell();

				if (useManaPotions)
					CheckManaAndUsePotion();

				if (refillRing)
					RefillRing();

				if (useFood)
					CheckHungerAndEatFood();

				// Sleep for a short while to avoid excessive CPU usage
				System.Threading.Thread.Sleep(500);  // Check every 500ms
			}
		}

		// Check the health bar and use a potion if health is low
		static void CheckHealthAndUsePotion()
		{
			Color healthColor = GetPixelColor(healthPotPos);

			// Skip action if we can't read the screen (RGB 0, 0, 0)
			if (healthColor.R == 0 && healthColor.G == 0 && healthColor.B == 0)
			{
				Console.WriteLine("Screen not readable, skipping health check.");
				return;
			}

			// If health color is NOT RGB(0, 175, 0), use health potion
			if (!(healthColor.R == 0 && healthColor.G == 175 && healthColor.B == 0))
			{
				inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F1); // Simulate health potion key press (F1)
				Console.WriteLine("Using health potion!");
			}
		}

		// Check the health bar and use a spell if health is low
		static void CheckHealthAndUseSpell()
		{
			Color healthSpellColor = GetPixelColor(healthSpellPos);

			// Skip action if we can't read the screen (RGB 0, 0, 0)
			if (healthSpellColor.R == 0 && healthSpellColor.G == 0 && healthSpellColor.B == 0)
			{
				Console.WriteLine("Screen not readable, skipping healing spell.");
				return;
			}

			// If health spell color is NOT RGB(0, 175, 0), use health spell
			if (!(healthSpellColor.R == 0 && healthSpellColor.G == 175 && healthSpellColor.B == 0))
			{
				inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F4); // Simulate health spell key press (F4)
				Console.WriteLine("Casting health spell!");
			}
		}

		// Check the mana bar and use a mana potion if mana is low
		static void CheckManaAndUsePotion()
		{
			Color manaPotionColor = GetPixelColor(manaPotionPos);

			// Skip action if we can't read the screen (RGB 0, 0, 0)
			if (manaPotionColor.R == 0 && manaPotionColor.G == 0 && manaPotionColor.B == 0)
			{
				Console.WriteLine("Screen not readable, skipping mana check.");
				return;
			}

			// If mana potion color is NOT RGB(0, 56, 116), use mana potion
			if (!(manaPotionColor.R == 0 && manaPotionColor.G == 56 && manaPotionColor.B == 116))
			{
				inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F2); // Simulate mana potion key press (F2)
				Console.WriteLine("Using mana potion!");
			}
		}

		// Refill ring if it is missing
		static void RefillRing()
		{
			Color ringColor = GetPixelColor(ringSlotPos);

			// Skip action if we can't read the screen (RGB 0, 0, 0)
			if (ringColor.R == 0 && ringColor.G == 0 && ringColor.B == 0)
			{
				Console.WriteLine("Screen not readable, skipping ring refill.");
				return;
			}

			// If ring slot color is RGB(84, 86, 89), equip ring
			if (ringColor.R == 84 && ringColor.G == 86 && ringColor.B == 89)
			{
				inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F3); // Simulate ring equip key press (F3)
				Console.WriteLine("Equipping ring!");
			}
		}

		// Check mana status and cast spell if mana is full
		static void CheckManaAndCastSpell()
		{
			Color manaFullColor = GetPixelColor(manaFullPos);

			// Skip action if we can't read the screen (RGB 0, 0, 0)
			if (manaFullColor.R == 0 && manaFullColor.G == 0 && manaFullColor.B == 0)
			{
				Console.WriteLine("Screen not readable, skipping mana cast.");
				return;
			}

			// If mana full color is NOT RGB(46, 46, 46), cast a spell
			if (!(manaFullColor.R == 46 && manaFullColor.G == 46 && manaFullColor.B == 46))
			{
				inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F9); // Simulate mana spell key press (F9)
				Console.WriteLine("Casting mana spell!");
			}
		}

		// Check hunger icon and eat food if hungry
		static void CheckHungerAndEatFood()
		{
			Color hungerColor = GetPixelColor(eatFoodPos);

			// Skip action if we can't read the screen (RGB 0, 0, 0)
			if (hungerColor.R == 0 && hungerColor.G == 0 && hungerColor.B == 0)
			{
				Console.WriteLine("Screen not readable, skipping food check.");
				return;
			}

			// If hunger icon color is RGB(243, 203, 128), eat food
			if (hungerColor.R == 243 && hungerColor.G == 203 && hungerColor.B == 128)
			{
				inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F10); // Simulate eating food key press (F10)
				Console.WriteLine("Eating food!");
			}
		}

		// Function to log and handle RGB 0, 0, 0 detection
		static void LogPixelColor(string elementName, Point position)
		{
			Color pixelColor = GetPixelColor(position);

			if (pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0)
			{
				Console.ForegroundColor = ConsoleColor.Red;  // Highlight in red for visibility
				Console.WriteLine($"{elementName}: Screen read failure (RGB: 0, 0, 0). Skipping this cycle.");
				Console.ResetColor();

				// Play an alert sound to notify the user
				SystemSounds.Exclamation.Play();
				return;
			}

			Console.WriteLine($"{elementName}: Position ({position.X}, {position.Y}), RGB ({pixelColor.R}, {pixelColor.G}, {pixelColor.B})");
		}

		// Capture the color of a single pixel at the specified position
		static Color GetPixelColor(Point position)
		{
			using (Bitmap screenshot = new Bitmap(1, 1))
			{
				using (Graphics g = Graphics.FromImage(screenshot))
				{
					g.CopyFromScreen(position, Point.Empty, new Size(1, 1));
				}
				return screenshot.GetPixel(0, 0);
			}
		}
	}
}