open System
open System.Windows.Forms


// Book record type
type Book =
    { Title: string
      Author: string
      Genre: string
      IsBorrowed: bool
      BorrowDate: DateTime option
      Borrower: string }


// Initialize the library's book collection as a mutable map
let mutable libraryBooks = Map.empty<string, Book>


// Refresh all books in a DataGridView
let displayBooks (dataGridView: DataGridView) =
    dataGridView.Rows.Clear()

    libraryBooks
    |> Map.iter (fun _ book ->
        let status = if book.IsBorrowed then "Borrowed" else "Available"
        let row = dataGridView.Rows.Add()
        dataGridView.Rows.[row].Cells.["Title"].Value <- book.Title
        dataGridView.Rows.[row].Cells.["Author"].Value <- book.Author
        dataGridView.Rows.[row].Cells.["Genre"].Value <- book.Genre
        dataGridView.Rows.[row].Cells.["Status"].Value <- status
        dataGridView.Rows.[row].Cells.["Borrower"].Value <- book.Borrower)
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Add a new book
let addBook title author genre =
    if String.IsNullOrWhiteSpace(title) || String.IsNullOrWhiteSpace(author) || String.IsNullOrWhiteSpace(genre) then
        MessageBox.Show("Please fill in all fields (Title, Author, Genre).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        |> ignore
    elif libraryBooks.ContainsKey(title) then
        MessageBox.Show("A book with this title already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        |> ignore
    else
        let newBook =
            { Title = title
              Author = author
              Genre = genre
              IsBorrowed = false
              BorrowDate = None
              Borrower = "" }

        libraryBooks <- libraryBooks.Add(title, newBook)

        MessageBox.Show("Book added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        |> ignore
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Search for a book
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
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Borrow a book
let borrowBook title borrower =
    match libraryBooks.TryFind title with
    | Some book when not book.IsBorrowed ->
        let updatedBook =
            { book with
                IsBorrowed = true
                BorrowDate = Some DateTime.Now
                Borrower = borrower }

        libraryBooks <- libraryBooks.Add(title, updatedBook)

        MessageBox.Show("Book borrowed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        |> ignore
    | Some _ -> 
        MessageBox.Show("The book is already borrowed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        |> ignore
    | None -> 
        MessageBox.Show("Book not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        |> ignore
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


let deleteBook title =
    if libraryBooks.ContainsKey(title) then
        libraryBooks <- libraryBooks.Remove(title)
        MessageBox.Show("Book deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        |> ignore
    else
        MessageBox.Show("Book not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        |> ignore
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Return a book
let returnBook title =
    match libraryBooks.TryFind title with
    | Some book when book.IsBorrowed ->
        let updatedBook =
            { book with
                IsBorrowed = false
                BorrowDate = None
                Borrower = "" }

        libraryBooks <- libraryBooks.Add(title, updatedBook)

        MessageBox.Show("Book returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        |> ignore
    | Some _ -> 
        MessageBox.Show("The book is already available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        |> ignore
    | None -> 
        MessageBox.Show("Book not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        |> ignore
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Windows Forms UI
let form = new Form(Text = "Library Management System", Width = 600, Height = 600)

// UI Elements
let titleLabel = new Label(Text = "Title:", Top = 20, Left = 20)
let authorLabel = new Label(Text = "Author:", Top = 60, Left = 20)
let genreLabel = new Label(Text = "Genre:", Top = 100, Left = 20)


let authorInput = new TextBox(Top = 60, Left = 120, Width = 200)
let titleInput = new TextBox(Top = 20, Left = 120, Width = 200)
let genreInput = new TextBox(Top = 100, Left = 120, Width = 200)

let borrowerLabel = new Label(Text = "Borrower:", Top = 510, Left = 20)
let borrowerInput = new TextBox(Top = 510, Left = 120, Width = 200)
let borrowButton = new Button(Text = "Borrow", Top = 510, Left = 380)
let returnButton = new Button(Text = "Return", Top = 510, Left = 480)

let addButton = new Button(Text = "Add", Top = 150, Left = 20)
let deleteButton = new Button(Text = "Delete", Top = 150, Left = 120)

let searchInput = new TextBox(Top = 150, Left = 320, Width = 150)
let searchButton = new Button(Text = "Search", Top = 150, Left = 483)


// DataGridView for displaying books
let dataGridView = new DataGridView(Top = 190, Left = 20, Width = 540, Height = 300)
dataGridView.ColumnCount <- 5
dataGridView.Columns.[0].Name <- "Title"
dataGridView.Columns.[1].Name <- "Author"
dataGridView.Columns.[2].Name <- "Genre"
dataGridView.Columns.[3].Name <- "Borrower"
dataGridView.Columns.[4].Name <- "Status"


// Event Handlers
dataGridView.CellClick.Add(fun args ->
    if args.RowIndex >= 0 then
        let row = dataGridView.Rows.[args.RowIndex]
        if row.Cells.["Title"].Value <> null then
            titleInput.Text <- row.Cells.["Title"].Value.ToString()
        else
            titleInput.Text <- ""
        
        if row.Cells.["Author"].Value <> null then
            authorInput.Text <- row.Cells.["Author"].Value.ToString()
        else
            authorInput.Text <- ""
        
        if row.Cells.["Genre"].Value <> null then
            genreInput.Text <- row.Cells.["Genre"].Value.ToString()
        else
            genreInput.Text <- ""

        if row.Cells.["Borrower"].Value <> null then
            borrowerInput.Text <- row.Cells.["Borrower"].Value.ToString()
        else
            borrowerInput.Text <- ""
)
addButton.Click.Add(fun _ ->
    addBook titleInput.Text authorInput.Text genreInput.Text
    displayBooks dataGridView
)
borrowButton.Click.Add(fun _ ->
    borrowBook titleInput.Text borrowerInput.Text
    displayBooks dataGridView
)
returnButton.Click.Add(fun _ ->
    returnBook titleInput.Text
    displayBooks dataGridView
)
searchButton.Click.Add(fun _ -> searchBook searchInput.Text)
deleteButton.Click.Add(fun _ ->
    deleteBook titleInput.Text
    displayBooks dataGridView
)


// Add UI Elements to the Form
form.Controls.AddRange(
    [| titleLabel
       titleInput
       authorLabel
       authorInput
       genreLabel
       genreInput
       borrowerLabel
       borrowerInput
       addButton
       searchButton
       borrowButton
       deleteButton
       returnButton
       searchInput
       dataGridView |]
)


// Run the Application
[<EntryPoint>]
let main _ =
    Application.Run(form)
    0
