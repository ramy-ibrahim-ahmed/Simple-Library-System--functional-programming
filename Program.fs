open System
open System.Windows.Forms

// Define the Book record type
type Book =
    { Title: string
      Author: string
      Genre: string
      IsBorrowed: bool
      BorrowDate: DateTime option }

// Initialize the library's book collection as a mutable map
let mutable libraryBooks = Map.empty<string, Book>

// Function to display all books in a DataGridView
let displayBooks (dataGridView: DataGridView) =
    // Clear the existing data in the grid
    dataGridView.Rows.Clear()

    // Populate the DataGridView with the books and their statuses
    libraryBooks
    |> Map.iter (fun _ book ->
        let status = if book.IsBorrowed then "Borrowed" else "Available"
        let row = dataGridView.Rows.Add()
        dataGridView.Rows.[row].Cells.["Title"].Value <- book.Title
        dataGridView.Rows.[row].Cells.["Author"].Value <- book.Author
        dataGridView.Rows.[row].Cells.["Genre"].Value <- book.Genre
        dataGridView.Rows.[row].Cells.["Status"].Value <- status)

// Function to add a new book
let addBook title author genre =
    if libraryBooks.ContainsKey(title) then
        MessageBox.Show("A book with this title already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        |> ignore
    else
        let newBook =
            { Title = title
              Author = author
              Genre = genre
              IsBorrowed = false
              BorrowDate = None }

        libraryBooks <- libraryBooks.Add(title, newBook)

        MessageBox.Show("Book added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        |> ignore

// Function to search for a book
let searchBook title =
    match libraryBooks.TryFind title with
    | Some book ->
        let status = if book.IsBorrowed then "Borrowed" else "Available"

        MessageBox.Show(
            $"Title: {book.Title}\nAuthor: {book.Author}\nGenre: {book.Genre}\nStatus: {status}",
            "Book Found",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        )
        |> ignore
    | None ->
        MessageBox.Show("Book not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        |> ignore

// Function to borrow a book
let borrowBook title =
    match libraryBooks.TryFind title with
    | Some book when not book.IsBorrowed ->
        let updatedBook =
            { book with
                IsBorrowed = true
                BorrowDate = Some DateTime.Now }

        libraryBooks <- libraryBooks.Add(title, updatedBook)

        MessageBox.Show("Book borrowed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        |> ignore
    | Some _ ->
        MessageBox.Show("The book is already borrowed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        |> ignore
    | None ->
        MessageBox.Show("Book not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        |> ignore

// Function to return a book
let returnBook title =
    match libraryBooks.TryFind title with
    | Some book when book.IsBorrowed ->
        let updatedBook =
            { book with
                IsBorrowed = false
                BorrowDate = None }

        libraryBooks <- libraryBooks.Add(title, updatedBook)

        MessageBox.Show("Book returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        |> ignore
    | Some _ ->
        MessageBox.Show("The book is already available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        |> ignore
    | None ->
        MessageBox.Show("Book not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        |> ignore


// Set up the Windows Forms UI
let form = new Form(Text = "Library Management System", Width = 600, Height = 600) // Increased form width

// UI Elements
let titleLabel = new Label(Text = "Title:", Top = 20, Left = 20)
let titleInput = new TextBox(Top = 20, Left = 150, Width = 300)

let authorLabel = new Label(Text = "Author:", Top = 60, Left = 20)
let authorInput = new TextBox(Top = 60, Left = 150, Width = 300)

let genreLabel = new Label(Text = "Genre:", Top = 100, Left = 20)
let genreInput = new TextBox(Top = 100, Left = 150, Width = 300)

let addButton = new Button(Text = "Add", Top = 160, Left = 50)
let searchButton = new Button(Text = "Search", Top = 160, Left = 150)
let borrowButton = new Button(Text = "Borrow", Top = 160, Left = 250)
let returnButton = new Button(Text = "Return", Top = 160, Left = 350)
let displayButton = new Button(Text = "Refresh", Top = 160, Left = 450)

// DataGridView for displaying books
let dataGridView = new DataGridView(Top = 220, Left = 20, Width = 540, Height = 300)
dataGridView.ColumnCount <- 5
dataGridView.Columns.[0].Name <- "Title"
dataGridView.Columns.[1].Name <- "Author"
dataGridView.Columns.[2].Name <- "Genre"
dataGridView.Columns.[3].Name <- "Status"

// Event Handlers
addButton.Click.Add(fun _ -> addBook titleInput.Text authorInput.Text genreInput.Text)
searchButton.Click.Add(fun _ -> searchBook titleInput.Text)
borrowButton.Click.Add(fun _ -> borrowBook titleInput.Text)
returnButton.Click.Add(fun _ -> returnBook titleInput.Text)
displayButton.Click.Add(fun _ -> displayBooks dataGridView)

// Add UI Elements to the Form
form.Controls.AddRange(
    [| titleLabel
       titleInput
       authorLabel
       authorInput
       genreLabel
       genreInput
       addButton
       searchButton
       borrowButton
       returnButton
       displayButton
       dataGridView |]
)

// Run the Application
[<EntryPoint>]
let main _ =
    Application.Run(form)
    0
