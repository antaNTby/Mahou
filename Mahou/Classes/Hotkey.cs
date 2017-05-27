﻿using System;

namespace Mahou
{
	public class Hotkey
	{
		public readonly int ID;
		public readonly uint VirtualKeyCode;
		public readonly uint Modifiers;
		public readonly bool Enabled, Double;
		/// <summary>
		/// Initializes an hotkey, modifs are ctrl(2), shift(4), alt(1), win(8).
		/// </summary>
		public Hotkey(bool enabled, uint VKCode, uint modifiers, int id, bool dble = false) {
			this.ID = id;
			this.Enabled = enabled;
			this.VirtualKeyCode = VKCode;
			this.Modifiers = modifiers;
			this.Double = dble;
		}
		/// <summary>
		/// All Mahou hotkey names and IDs.
		/// </summary>
		public enum HKID {
			ConvertLastWord,
			ConvertSelection,
			ConvertLastLine,
			ConvertMultipleWords,
			ToTitleSelection,
			ToSwapSelection,
			ToRandomSelection,
			TransliterateSelection,
			ToggleSymbolIgnoreMode,
			ToggleVisibility,
			Exit,
			Restart
		}
		/// <summary>
		/// Gets all modifiers in hotkey.
		/// </summary>
		/// <param name="hkmods">Hotkey modifiers string.</param>
		public static uint GetMods(string hkmods) {
			uint MOD = 0; // No repeat for hotkey actions
			if (hkmods.Contains("Alt"))
				MOD += WinAPI.MOD_ALT;
			if (hkmods.Contains("Control"))
				MOD += WinAPI.MOD_CONTROL;
			if (hkmods.Contains("Shift"))
				MOD += WinAPI.MOD_SHIFT;
			if( hkmods.Contains("Win"))
				MOD += WinAPI.MOD_WIN;
			return MOD;
		}
		/// <summary>
		/// Checks if modifiers "mods" contains modifier "mod".
		/// </summary>
		/// <returns>True if "mods" contains "mod".</returns>
		public static bool ContainsModifier(int mods, int mod) {
			if (mod == WinAPI.MOD_WIN && mods >= WinAPI.MOD_WIN) {
				return true;
			}
			if (mod == WinAPI.MOD_SHIFT && (mods == WinAPI.MOD_SHIFT || 
			    mods == WinAPI.MOD_SHIFT + WinAPI.MOD_ALT ||
			    mods == WinAPI.MOD_SHIFT + WinAPI.MOD_CONTROL ||
			    mods == WinAPI.MOD_SHIFT + WinAPI.MOD_WIN || 
			    mods == WinAPI.MOD_SHIFT + WinAPI.MOD_ALT + WinAPI.MOD_CONTROL ||
			    mods == WinAPI.MOD_SHIFT + WinAPI.MOD_WIN + WinAPI.MOD_CONTROL ||
			   	mods == WinAPI.MOD_SHIFT + WinAPI.MOD_WIN + WinAPI.MOD_ALT ||
			   	mods == WinAPI.MOD_SHIFT + WinAPI.MOD_WIN + WinAPI.MOD_CONTROL + WinAPI.MOD_ALT)) {
				return true;
			}
			if (mod == WinAPI.MOD_CONTROL && (mods == WinAPI.MOD_CONTROL || 
			    mods == WinAPI.MOD_CONTROL + WinAPI.MOD_SHIFT ||
			    mods == WinAPI.MOD_CONTROL + WinAPI.MOD_ALT ||
			    mods == WinAPI.MOD_CONTROL + WinAPI.MOD_WIN || 
			    mods == WinAPI.MOD_CONTROL + WinAPI.MOD_ALT + WinAPI.MOD_SHIFT ||
			    mods == WinAPI.MOD_CONTROL + WinAPI.MOD_WIN + WinAPI.MOD_SHIFT ||
			   	mods == WinAPI.MOD_CONTROL + WinAPI.MOD_WIN + WinAPI.MOD_ALT ||
			   	mods == WinAPI.MOD_CONTROL + WinAPI.MOD_WIN + WinAPI.MOD_SHIFT + WinAPI.MOD_ALT)) {
				return true;
			}
			if (mod == WinAPI.MOD_ALT && mods % 2 != 0) {
				return true;
			}
			return false;
		}
		public static void CallHotkey(Hotkey hotkey, HKID hkID, ref bool hkOK, Action hkAction) {
			if (!hotkey.Double) hkOK = true;
			if (hotkey.ID == (int)hkID && hotkey.Enabled) {
				if (hkOK) {
					Logging.Log("Hotkey [" + Enum.GetName(typeof(HKID), hkID) + "] fired.");
					if (MMain.mahou.BlockHKWithCtrl && ContainsModifier((int)hotkey.Modifiers, (int)WinAPI.MOD_CONTROL)) { } else {
						if (hotkey.Modifiers > 0 && MMain.mahou.RePress) {
							KMHook.hotkeywithmodsfired = true;
							KMHook.RePressAfter(Convert.ToInt32(hotkey.Modifiers));
						}
					}
					KMHook.SendModsUp(Convert.ToInt32(hotkey.Modifiers));
					KMHook.IfKeyIsMod((System.Windows.Forms.Keys)hotkey.VirtualKeyCode);
					hkAction();
					KMHook.RePress();
				} else if (hotkey.Double) {
					Logging.Log("Waiting for second hotkey ["+Enum.GetName(typeof(HKID), hkID) +"] press.");
					hkOK = true;
					KMHook.doublekey.Interval = MMain.mahou.DoubleHKInterval;
					KMHook.doublekey.Start();
			}
			}
		}
	}
}
