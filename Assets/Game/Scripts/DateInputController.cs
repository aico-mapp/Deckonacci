using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class DateInputController : MonoBehaviour
    {
        public TMP_InputField dateInputField;
        [SerializeField] private Button _addButton;
    
        private const string DateFormat = "dd.MM.yyyy";
        private DateTime _parsedDate;
        public DateTime ParsedDate => _parsedDate;
    
        // To track if we need to adjust cursor position
        private bool _needsPositionAdjustment;
        private int _targetCursorPosition;

        private void Start()
        {
            // Set placeholder text to show expected format
            dateInputField.placeholder.GetComponent<TextMeshProUGUI>().text = "DD.MM.YYYY";
        
            // Listen for input changes to format while typing
            dateInputField.onValueChanged.AddListener(FormatDateInput);
        
            // Disable button initially 
            if (_addButton != null)
                _addButton.interactable = false;
        }
    
        private void Update()
        {
            // Apply cursor position adjustment after the UI has updated
            if (_needsPositionAdjustment && dateInputField.isFocused)
            {
                dateInputField.caretPosition = _targetCursorPosition;
                dateInputField.selectionAnchorPosition = _targetCursorPosition;
                dateInputField.selectionFocusPosition = _targetCursorPosition;
                _needsPositionAdjustment = false;
            }
        }
    
        private void FormatDateInput(string text)
        {
            // Save the current cursor position
            int cursorPosition = dateInputField.caretPosition;
        
            // First, remove any non-digit characters the user might have entered
            string digitsOnly = System.Text.RegularExpressions.Regex.Replace(text, @"[^\d]", "");
        
            // Count dots in the text before the cursor
            int dotsBefore = 0;
            for (int i = 0; i < cursorPosition; i++)
            {
                if (i < text.Length && text[i] == '.')
                    dotsBefore++;
            }
        
            // Build the formatted string with dots at the right positions
            string formattedText = "";
            int newDotsBefore = 0;
        
            for (int i = 0; i < digitsOnly.Length && i < 8; i++) // Limit to 8 digits (DDMMYYYY)
            {
                // Add separator after day (2 digits) and month (2 digits)
                if (i == 2 || i == 4)
                {
                    formattedText += ".";
                    // If we're before or at the cursor position, count this new dot
                    if (i < cursorPosition - dotsBefore)
                        newDotsBefore++;
                }
                
                formattedText += digitsOnly[i];
            }
        
            // Only update if the text has changed
            if (formattedText != text)
            {
                // Calculate new cursor position
                int digitsBeforeCursor = cursorPosition - dotsBefore;
                if (digitsBeforeCursor < 0) digitsBeforeCursor = 0;
            
                // New cursor position = digits before cursor + new dots before cursor
                _targetCursorPosition = digitsBeforeCursor + newDotsBefore;
            
                // Ensure cursor position is within bounds
                if (_targetCursorPosition > formattedText.Length)
                    _targetCursorPosition = formattedText.Length;
            
                // Update text without triggering the event again
                dateInputField.SetTextWithoutNotify(formattedText);
            
                // Flag that cursor position needs adjustment
                _needsPositionAdjustment = true;
            }
        
            // Check date validity on each input change
            CheckDateValidity(formattedText);
        }
    
        private void CheckDateValidity(string input)
        {
            bool isValid = false;
        
            // Only consider input valid if it's a complete date (10 characters)
            if (input.Length == 10)
            {
                isValid = DateTime.TryParseExact(input, DateFormat, CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, out _parsedDate);
            
                if (isValid)
                {
                    Debug.Log($"Valid date: {_parsedDate}");
                }
            }
        
            // Update button interactability based on date validity
            if (_addButton != null)
            {
                _addButton.interactable = isValid;
            }
        }
    
        // Public method to get the parsed date, returns true if valid
        public bool TryGetDate(out DateTime date)
        {
            date = _parsedDate;
            return DateTime.TryParseExact(dateInputField.text, DateFormat, 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.None, out _);
        }
    }
}