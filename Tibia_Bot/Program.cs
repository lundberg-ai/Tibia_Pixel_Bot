using System.Drawing;
using System.Media;
using WindowsInput;
using WindowsInput.Native;

namespace Tibia_Bot
{
	class Program
	{
		// Configuration settings
		static int healthThreshold = 50; // Use health potion below 50% health
		static int manaThreshold = 50; // Use mana potion below 50% mana
									   // static int spellManaThreshold = 90; // Cast spell when mana is above 90%

		static Point healthBarPos = new Point(1813, 147);  // Full health bar position
		static Point manaBarPos = new Point(1812, 161);    // Full mana bar position
		static Point ringSlotPos = new Point(1768, 272);   // Ring slot when unequipped
		static Point hungerIconPos = new Point(1760, 321); // Hunger icon when hungry

		static bool useHealthPotions = true;
		static bool useManaPotions = true;
		static bool useHealingSpells = false;
		static bool refillRing = false;
		static bool useFood = true;

		// Input simulator for key presses
		static InputSimulator inputSimulator = new InputSimulator();

		static void Main(string[] args)
		{
			Console.WriteLine("Tibia bot started!");

			while (true)
			{
				// Log pixel colors for debugging during development
				LogPixelColor("Health Bar", healthBarPos);
				LogPixelColor("Mana Bar", manaBarPos);
				LogPixelColor("Ring Slot", ringSlotPos);
				LogPixelColor("Hunger Icon", hungerIconPos);

				// Main loop to keep checking conditions and performing actions
				if (useHealthPotions)
					CheckHealthAndUsePotion();

				if (useHealingSpells)
					UseHealingSpell();

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
			Color healthColor = GetPixelColor(healthBarPos);

			// Skip action if we can't read the screen (RGB 0, 0, 0)
			if (healthColor.R == 0 && healthColor.G == 0 && healthColor.B == 0)
			{
				Console.WriteLine("Screen not readable, skipping health check.");
				return;
			}

			// Assuming red color means low health, check if RGB matches low health condition
			if (healthColor.R < 200 && healthColor.G < 50 && healthColor.B < 50)  // Threshold for low health
			{
				inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F1); // Simulate health potion key press (F1)
				Console.WriteLine("Using health potion!");
			}
		}

		// Use healing spell if health is low and mana is available
		static void UseHealingSpell()
		{
			Color healthColor = GetPixelColor(healthBarPos);
			Color manaColor = GetPixelColor(manaBarPos);

			// Skip action if we can't read the screen (RGB 0, 0, 0)
			if ((healthColor.R == 0 && healthColor.G == 0 && healthColor.B == 0) || (manaColor.R == 0 && manaColor.G == 0 && manaColor.B == 0))
			{
				Console.WriteLine("Screen not readable, skipping healing spell.");
				return;
			}

			// Check if health is low and mana is high enough
			if (healthColor.R < 200 && manaColor.B > 200) // Low health and sufficient mana
			{
				inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F4); // Simulate healing spell key press (F4)
				Console.WriteLine("Casting healing spell!");
			}
		}

		// Check the mana bar and use a mana potion if mana is low
		static void CheckManaAndUsePotion()
		{
			Color manaColor = GetPixelColor(manaBarPos);

			// Skip action if we can't read the screen (RGB 0, 0, 0)
			if (manaColor.R == 0 && manaColor.G == 0 && manaColor.B == 0)
			{
				Console.WriteLine("Screen not readable, skipping mana check.");
				return;
			}

			// Assuming blue color means full mana, and darker blue means low mana
			if (manaColor.B < 150)  // Threshold for low mana
			{
				inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F2); // Simulate mana potion key press (F3)
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

			// Assuming ring slot is empty if it's a specific gray color
			if (ringColor.R == 54 && ringColor.G == 57 && ringColor.B == 60) // Ring unequipped
			{
				inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F3); // Simulate ring refill key press (F3)
				Console.WriteLine("Refilling ring!");
			}
		}

		// Check hunger icon and eat food if hungry
		static void CheckHungerAndEatFood()
		{
			Color hungerColor = GetPixelColor(hungerIconPos);

			// Skip action if we can't read the screen (RGB 0, 0, 0)
			if (hungerColor.R == 0 && hungerColor.G == 0 && hungerColor.B == 0)
			{
				Console.WriteLine("Screen not readable, skipping food check.");
				return;
			}

			// Check if the hunger icon is visible (yellowish color)
			if (hungerColor.R == 246 && hungerColor.G == 212 && hungerColor.B == 143) // Hungry
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