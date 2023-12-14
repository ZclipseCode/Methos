using GoogleTextToSpeech.Scripts;
using GoogleTextToSpeech.Scripts.Data;
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
    [SerializeField] Button nextButton;

    [SerializeField] List<Actor> actors;
    [SerializeField] string environment;
    [TextArea][SerializeField] string prompt = $"You must write a script for a short scene of Breaking Bad.";

    [SerializeField] TextToSpeech textToSpeech;
    [SerializeField] AudioSource audioSource;
    Dictionary<string, VoiceScriptableObject> namesWithVoices = new Dictionary<string, VoiceScriptableObject>();

    OpenAIAPI api;
    List<ChatMessage> messages;

    string[] scriptLines;
    int currentLineIndex;

    Action<AudioClip> _audioClipReceived;
    Action<BadRequestData> _errorReceived;

    List<ActorClass> actorClasses = new List<ActorClass>();

    private void Awake()
    {
        _errorReceived += ErrorReceived;
        _audioClipReceived += AudioClipReceived;
    }

    private void Start()
    {
        api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User));

        if (actors.Count < 2)
        {
            Debug.Log("Too few actors!");
            return;
        }

        for (int i = 0; i < actors.Count; i++)
        {
            actorClasses.Add(actors[i].GetActorClass());
        }

        AddPromptDetails();

        StartScript();

        sendButton.onClick.AddListener(() => GetResponse());
        nextButton.onClick.AddListener(() => NextLine());
    }

    void AddPromptDetails()
    {
        prompt += $" The format the script should be written is 'Character: What they say.' followed by a new line. You can not respond with anything other than the characters' lines. The setting is {environment}. The characters are: ";
        for (int i = 0; i < actors.Count - 1; i++)
        {
            prompt += $"{actorClasses[i].ActorName}, ";
        }
        prompt += $"and {actorClasses[actors.Count - 1].ActorName}.";
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
        // disable the send button
        sendButton.enabled = false;

        // send the entire chat to OpenAI to get the new message
        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.1,
            //MaxTokens = 1000,
            MaxTokens = 500,
            Messages = messages
        });

        // get the response message
        ChatMessage responseMessage = new ChatMessage();
        responseMessage.Role = chatResult.Choices[0].Message.Role;
        responseMessage.Content = chatResult.Choices[0].Message.Content;

        // add the response to the list of messages
        messages.Add(responseMessage);

        // create an array of the script's lines
        scriptLines = responseMessage.Content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        // get who the actors are
        // right now assumes actors talk in the same order
        for (int i = 0; i < actors.Count; i++)
        {
            string[] parts = scriptLines[i].Split(':');
            
            for (int j = 0; j < actors.Count; j++)
            {
                if (parts[0] == actorClasses[j].ActorName)
                {
                    namesWithVoices.Add(parts[0], actorClasses[j].Voice);
                }
            }
        }

        // display the first line
        NextLine();

        // send button never gets re-enabled
    }

    void NextLine()
    {
        if (currentLineIndex >= scriptLines.Length)
        {
            return;
        }

        textField.text = scriptLines[currentLineIndex];

        // splits string into two parts, one before the colon (name) and one after (message)
        string[] parts = scriptLines[currentLineIndex].Split(':');
        VoiceScriptableObject voice = namesWithVoices[parts[0]];
        textToSpeech.GetSpeechAudioFromGoogle(parts[1], voice, _audioClipReceived, _errorReceived); // voice should be on actor

        currentLineIndex++;
    }

    private void ErrorReceived(BadRequestData badRequestData)
    {
        Debug.Log($"Error {badRequestData.error.code} : {badRequestData.error.message}");
    }

    private void AudioClipReceived(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void OnDestroy()
    {
        _errorReceived -= ErrorReceived;
        _audioClipReceived -= AudioClipReceived;
    }
}
