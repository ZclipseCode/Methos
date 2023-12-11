using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenAIController : MonoBehaviour
{
    [SerializeField] TMP_Text textField;
    [SerializeField] Button sendButton;
    [SerializeField] List<Actor> actors;
    [SerializeField] string environment;
    [TextArea][SerializeField] string prompt = $"You must write a script for a short scene of Breaking Bad.";

    OpenAIAPI api;
    List<ChatMessage> messages;

    private void Start()
    {
        api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User));

        if (actors.Count < 2)
        {
            Debug.Log("Too few actors!");
            return;
        }

        AddPromptDetails();

        StartScript();

        sendButton.onClick.AddListener(() => GetResponse());
    }

    void AddPromptDetails()
    {
        prompt += $" The format the script should be written is 'Character: What they say.' followed by a new line. You can not respond with anything other than the characters' lines. The setting is {environment}. The characters are: ";
        for (int i = 0; i < actors.Count - 1; i++)
        {
            prompt += $"{actors[i].GetActorName()}, ";
        }
        prompt += $"and {actors[actors.Count - 1].GetActorName()}.";
    }

    void StartScript()
    {
        messages = new List<ChatMessage>
        {
            new ChatMessage(ChatMessageRole.System, prompt)
        };
    }

    async void GetResponse()
    {
        // disable the next button
        sendButton.enabled = false;

        // send the entire chat to OpenAI to get the new message
        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.1,
            MaxTokens = 1000,
            Messages = messages
        });

        // get the response message
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;

        // add the response to the list of messages
        messages.Add(responseMessage);

        // update the text field with the response
        textField.text = string.Format(responseMessage.Content);

        // re-enable the next button
        sendButton.enabled = true;
    }
}
