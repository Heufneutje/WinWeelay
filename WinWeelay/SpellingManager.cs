﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace WinWeelay
{
    /// <summary>
    /// Handles spelling suggestion context menus and manages a custom dictionary to which words can be added.
    /// </summary>
    public class SpellingManager
    {
        private readonly string _appDataPath;
        private string _mainDictPath;
        private string _customDictPath;
        private Uri _mainDictionary;
        private Uri _customDictionary;
        private readonly List<TextBoxBase> _subscribedTextBoxes;

        private bool _isEnabled;

        /// <summary>
        /// Whether the spell checker is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                RefreshSubscribedTextBoxes();
            }
        }

        private CultureInfo _language;

        /// <summary>
        /// The language to use for the spell checker.
        /// </summary>
        public CultureInfo Language
        {
            get => _language;
            set
            {
                _language = value;
                SetDictionaryPaths();
            }
        }

        /// <summary>
        /// Create a new instance of the manager. Initializes the dictionary from the lexicon file in the AppData folder.
        /// </summary>
        public SpellingManager()
        {
            _subscribedTextBoxes = new List<TextBoxBase>();
            _appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinWeelay");
            Language = Thread.CurrentThread.CurrentUICulture;
        }

        /// <summary>
        /// Read the dictionaries from the app data folder.
        /// </summary>
        public void SetDictionaryPaths()
        {
            _mainDictPath = Path.Combine(_appDataPath, $"{_language.Name}.lex");
            _customDictPath = Path.Combine(_appDataPath, "customdict.lex");

            if (File.Exists(_mainDictPath))
                _mainDictionary = new Uri(_mainDictPath);

            if (!File.Exists(_customDictPath))
                File.Create(_customDictPath);

            _customDictionary = new Uri(_customDictPath);
            RefreshSubscribedTextBoxes();
        }

        /// <summary>
        /// Check whether a dictionary exists for a given language
        /// </summary>
        /// <param name="language">The language to check.</param>
        /// <returns>Whether a dictionary exists for a given language></returns>
        public bool IsDictionaryInstalled(CultureInfo language) => File.Exists(Path.Combine(_appDataPath, $"{language.Name}.lex"));

        /// <summary>
        /// Install a dictionary for a given language.
        /// </summary>
        /// <param name="language">The language of the dictionary.</param>
        /// <param name="dictionaryPath">The path to copy the dictionary</param>
        public void InstallDictionary(CultureInfo language, string dictionaryPath)
        {
            File.Copy(dictionaryPath, Path.Combine(_appDataPath, $"{language.Name}.lex"), true);
        }

        /// <summary>
        /// Add a given word to the custom dictionary.
        /// </summary>
        /// <param name="word">The word to add.</param>
        public void AddWordToDictionary(string word)
        {
            File.AppendAllText(_customDictPath, $"{word}{Environment.NewLine}");
            RefreshSubscribedTextBoxes();
        }

        /// <summary>
        /// Subscribe a given text box to the spell checker. Enables spell checking, customizes the context menu to show suggestions and updates the spell checker when the custom diciontary is modified.
        /// </summary>
        /// <param name="textBox">The text box to subscribe.</param>
        public void Subscribe(TextBoxBase textBox)
        {
            IList dictionaries = SpellCheck.GetCustomDictionaries(textBox);
            dictionaries.Add(_customDictionary);

            if (_mainDictionary != null)
                dictionaries.Add(_mainDictionary);

            textBox.SpellCheck.IsEnabled = _isEnabled;
            textBox.ContextMenuOpening += TextBox_ContextMenuOpening;

            if (!_subscribedTextBoxes.Contains(textBox))
                _subscribedTextBoxes.Add(textBox);
        }

        /// <summary>
        /// Unsubscribe a given text box from the spell checker.
        /// </summary>
        /// <param name="textBox">The text box to unsubscribe.</param>
        public void Unsubscribe(TextBoxBase textBox)
        {
            IList dictionaries = SpellCheck.GetCustomDictionaries(textBox);
            dictionaries.Clear();

            textBox.SpellCheck.IsEnabled = false;
            textBox.ContextMenuOpening -= TextBox_ContextMenuOpening;

            if (_subscribedTextBoxes.Contains(textBox))
                _subscribedTextBoxes.Remove(textBox);
        }

        private void RefreshSubscribedTextBoxes()
        {
            foreach (TextBoxBase textBox in _subscribedTextBoxes.ToArray())
            {
                Unsubscribe(textBox);
                Subscribe(textBox);
            }
        }

        private void TextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            TextBoxBase textBoxBase = (TextBoxBase)sender;
            const string controlTag = "spellcheck";

            int cmdIndex = 0;
            SpellingError spellingError;
            string misspelledWord;

            foreach (Control item in textBoxBase.ContextMenu.Items.Cast<Control>().ToArray())
                if ((string)item.Tag == controlTag)
                    textBoxBase.ContextMenu.Items.Remove(item);

            if (textBoxBase is TextBox textBox)
            {
                int caretIndex = textBox.CaretIndex;
                spellingError = textBox.GetSpellingError(caretIndex);
                if (spellingError == null)
                    return;

                misspelledWord = textBox.Text.Substring(textBox.GetSpellingErrorStart(caretIndex), textBox.GetSpellingErrorLength(caretIndex));
            }
            else if (textBoxBase is RichTextBox richTextBox)
            {
                TextPointer caretPosition = richTextBox.CaretPosition;
                spellingError = richTextBox.GetSpellingError(caretPosition);
                if (spellingError == null)
                    return;

                misspelledWord = richTextBox.GetWordByCaret();
            }
            else
                return;

            MenuItem spellingItem = new()
            {
                Header = "Spelling",
                Tag = controlTag
            };
            textBoxBase.ContextMenu.Items.Insert(0, spellingItem);
            textBoxBase.ContextMenu.Items.Insert(1, new Separator() { Tag = controlTag });

            foreach (string str in spellingError.Suggestions)
            {
                MenuItem item = new()
                {
                    Header = str,
                    FontWeight = FontWeights.Bold,
                    Command = EditingCommands.CorrectSpellingError,
                    CommandParameter = str,
                    CommandTarget = textBoxBase,
                    Tag = controlTag
                };
                spellingItem.Items.Insert(cmdIndex, item);
                cmdIndex++;
            }

            if (spellingError.Suggestions.Any())
            {
                spellingItem.Items.Insert(cmdIndex, new Separator() { Tag = controlTag });
                cmdIndex++;
            }

            MenuItem ignoreAllItem = new()
            {
                Header = "Ignore All",
                Command = EditingCommands.IgnoreSpellingError,
                CommandTarget = textBoxBase,
                Tag = controlTag
            };
            spellingItem.Items.Insert(cmdIndex, ignoreAllItem);

            MenuItem addToDictionarylItem = new()
            {
                Header = "Add to Dictionary",
                Tag = controlTag
            };
            addToDictionarylItem.Click += delegate { AddWordToDictionary(misspelledWord); };

            spellingItem.Items.Insert(cmdIndex, addToDictionarylItem);
        }
    }
}
