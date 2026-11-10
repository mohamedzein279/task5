using System;
using System.Collections.Generic;
using System.Linq;

interface IQuestion
{
    void DisplayQuestion();
    bool CheckAnswer(string answer);
}


class Question
{
    public string Header { get; set; }
    public int Marks { get; set; }
    public string Level { get; set; }

    public Question(string header, int marks, string level)
    {
        Header = header;
        Marks = marks;
        Level = level;
    }
}



class TrueFalseQuestion : Question, IQuestion
{
    public bool CorrectAnswer { get; set; }

    public TrueFalseQuestion(string header, int marks,
        string level, bool correctAnswer)
        : base(header, marks, level)
    {
        CorrectAnswer = correctAnswer;
    }

    public void DisplayQuestion()
    {
        Console.WriteLine(Header);
        Console.WriteLine("1. True");
        Console.WriteLine("2. False");
    }

    public bool CheckAnswer(string answer)
    {
        return answer == (CorrectAnswer ? "1" : "2");
    }
}


class ChooseOneQuestion : Question, IQuestion
{
    public List<string> Choices { get; set; }
    public int CorrectAnswer { get; set; }

    public ChooseOneQuestion(string header, int marks,
        string level, List<string> choices,
        int correctAnswer)
        : base(header, marks, level)
    {
        Choices = choices;
        CorrectAnswer = correctAnswer;
    }

    public void DisplayQuestion()
    {
        Console.WriteLine(Header);

        for (int i = 0; i < Choices.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {Choices[i]}");
        }
    }

    public bool CheckAnswer(string answer)
    {
        return int.TryParse(answer, out int result)
               && result == CorrectAnswer;
    }
}



class MultipleChoiceQuestion : Question, IQuestion
{
    public List<string> Choices { get; set; }
    public List<int> CorrectAnswers { get; set; }

    public MultipleChoiceQuestion(string header, int marks,
        string level, List<string> choices,
        List<int> correctAnswers)
        : base(header, marks, level)
    {
        Choices = choices;
        CorrectAnswers = correctAnswers;
    }

    public void DisplayQuestion()
    {
        Console.WriteLine(Header);

        for (int i = 0; i < Choices.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {Choices[i]}");
        }

        Console.WriteLine(
            "Enter correct choices separated by commas:");
    }

    public bool CheckAnswer(string answer)
    {
        try
        {
            List<int> studentAnswers = answer
                .Split(',')
                .Select(x => int.Parse(x.Trim()))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            List<int> correct = CorrectAnswers
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return studentAnswers.SequenceEqual(correct);
        }
        catch
        {
            return false;
        }
    }
}


class Program
{
    static List<Question> QuestionBank =
        new List<Question>();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine(" EXAMINATION SYSTEM ");
            Console.WriteLine("1. Doctor Mode");
            Console.WriteLine("2. Student Mode");
            Console.WriteLine("3. Exit");

            Console.Write("Choose: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DoctorMode();
                    break;

                case "2":
                    StudentMode();
                    break;

                case "3":
                    Console.WriteLine("Goodbye!");
                    return;

                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        }
    }



    static void DoctorMode()
    {
        Console.Write("Enter number of questions to add: ");

        int count = ReadPositiveNumber();

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"Question {i + 1}");

            Console.WriteLine("1. True/False");
            Console.WriteLine("2. Choose One");
            Console.WriteLine("3. Multiple Choice");

            Console.Write("Select question type: ");

            string type = ReadOption(1, 3);

            Console.WriteLine("Select Level:");
            Console.WriteLine("1. Easy");
            Console.WriteLine("2. Medium");
            Console.WriteLine("3. Hard");

            Console.Write("Level: ");

            string levelChoice = ReadOption(1, 3);

            string level = "";

            if (levelChoice == "1")
                level = "Easy";

            else if (levelChoice == "2")
                level = "Medium";

            else
                level = "Hard";

            Console.Write("Enter question header: ");

            string header = Console.ReadLine();

            Console.Write("Enter marks: ");

            int marks = ReadPositiveNumber();

            switch (type)
            {
                case "1":

                    Console.WriteLine("Correct Answer:");
                    Console.WriteLine("1. True");
                    Console.WriteLine("2. False");

                    int tfAnswer = ReadNumber(1, 2);

                    QuestionBank.Add(
                        new TrueFalseQuestion(
                            header,
                            marks,
                            level,
                            tfAnswer == 1
                        )
                    );

                    break;

                case "2":

                    List<string> choices1 = GetChoices();

                    Console.Write("Enter correct choice number: ");

                    int correctChoice = ReadNumber(1, 4);

                    QuestionBank.Add(
                        new ChooseOneQuestion(
                            header,
                            marks,
                            level,
                            choices1,
                            correctChoice
                        )
                    );

                    break;

                case "3":

                    List<string> choices2 = GetChoices();

                    Console.Write(
                        "Enter correct answers separated by commas: ");

                    List<int> correctAnswers = ReadCorrectAnswers();

                    QuestionBank.Add(
                        new MultipleChoiceQuestion(
                            header,
                            marks,
                            level,
                            choices2,
                            correctAnswers
                        )
                    );

                    break;
            }

            Console.WriteLine("Question added successfully!");
        }

        Console.WriteLine("All questions added successfully!");
    }


    static List<string> GetChoices()
    {
        List<string> choices = new List<string>();

        for (int i = 0; i < 4; i++)
        {
            Console.Write($"Enter choice {i + 1}: ");

            choices.Add(Console.ReadLine());
        }

        return choices;
    }



    static List<int> ReadCorrectAnswers()
    {
        while (true)
        {
            try
            {
                List<int> answers = Console.ReadLine()
                    .Split(',')
                    .Select(x => int.Parse(x.Trim()))
                    .Distinct()
                    .ToList();

                if (answers.Count > 0 &&
                    answers.All(x => x >= 1 && x <= 4))
                {
                    return answers;
                }
            }
            catch
            {
            }

            Console.WriteLine(
                "Invalid input! Enter numbers from 1 to 4:");
        }
    }


    static void StudentMode()
    {
        if (QuestionBank.Count == 0)
        {
            Console.WriteLine("No questions available!");
            return;
        }

        Console.WriteLine("Select Exam Type:");
        Console.WriteLine("1. Practical");
        Console.WriteLine("2. Final");

        Console.Write("Choose: ");

        string examChoice = ReadOption(1, 2);

        string examType = "";

        if (examChoice == "1")
            examType = "Practical";

        else
            examType = "Final";

        Console.WriteLine("Select Exam Level:");
        Console.WriteLine("1. Easy");
        Console.WriteLine("2. Medium");
        Console.WriteLine("3. Hard");

        Console.Write("Choose: ");

        string levelChoice = ReadOption(1, 3);

        string level = "";

        if (levelChoice == "1")
            level = "Easy";

        else if (levelChoice == "2")
            level = "Medium";

        else
            level = "Hard";

        List<Question> examQuestions = QuestionBank
            .Where(q => q.Level == level)
            .ToList();

        if (examQuestions.Count == 0)
        {
            Console.WriteLine(
                "No questions found for this level!");

            return;
        }

        Console.WriteLine(" EXAM STARTED ");

        Console.WriteLine($"Exam Type: {examType}");
        Console.WriteLine($"Level: {level}");

        int score = 0;
        int totalMarks = 0;

        for (int i = 0; i < examQuestions.Count; i++)
        {
            Question q = examQuestions[i];

            // Polymorphism using Interface

            IQuestion question = (IQuestion)q;

            Console.WriteLine($"\nQuestion {i + 1}");
            Console.WriteLine($"Marks: {q.Marks}");

            question.DisplayQuestion();

            Console.Write("Your Answer: ");

            string answer = Console.ReadLine();

            if (question.CheckAnswer(answer))
            {
                Console.WriteLine("Correct Answer!");

                score += q.Marks;
            }
            else
            {
                Console.WriteLine("Wrong Answer!");
            }

            totalMarks += q.Marks;
        }

        Console.WriteLine(" EXAM RESULT");

        Console.WriteLine($"Your Score: {score}");

        Console.WriteLine($"Total Marks: {totalMarks}");

        Console.WriteLine(
            $"Percentage: {(double)score / totalMarks * 100:F2}%"
        );

        Console.WriteLine("EXAM FINISHED ");
    }


    static string ReadOption(int min, int max)
    {
        while (true)
        {
            string input = Console.ReadLine();

            if (int.TryParse(input, out int number) &&
                number >= min && number <= max)
            {
                return input;
            }

            Console.Write(
                $"Enter a number between {min} and {max}: ");
        }
    }

    static int ReadNumber(int min, int max)
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int number)
                && number >= min && number <= max)
            {
                return number;
            }

            Console.Write(
                $"Enter a number between {min} and {max}: ");
        }
    }

    static int ReadPositiveNumber()
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int number)
                && number > 0)
            {
                return number;
            }

            Console.Write("Enter a positive number: ");
        }
    }
}
