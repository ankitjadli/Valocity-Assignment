// Utility function for DOM selection
const $ = (id) => document.getElementById(id);

// DOM Elements
const DOM = {
    startBtn: $("start-btn"),
    sequenceDisplay: $("sequence-display"),
    cardGrid: $("card-grid"),
    sequenceCards: $("sequence-cards"),
    scoreDisplay: $("score-value"),
    gameInstruction: $("game-rules"),
    submitButton: $("submitButton"),
    resetButton: $("resetButton"),
    scoreDiv: $("score"),
    userLostGrid: $("user-lost-grid"),
    correctCardGrid: $("card-grid-correct"),
    incorrectCardGrid: $("card-grid-user")
};

// Global Variables
const GameState = {
    gameId: "",
    currentLevel: 1,
    userSequence: [],
    correctSequence: [],
    score: 0
};

// Initialize Game
const initGame = () => {
    resetGame();
    attachEventListeners();
};

// Attach Event Listeners
const attachEventListeners = () => {
    DOM.resetButton.addEventListener("click", resetGame);
    DOM.startBtn.addEventListener("click", startGame);
    DOM.submitButton.addEventListener("click", submitSequence);
};

// Reset Game State
const resetGame = () => {
    Object.assign(GameState, {
        gameId: "",
        currentLevel: 1,
        userSequence: [],
        correctSequence: [],
        score: 0
    });

    updateUI({
        startBtnVisible: true,
        scoreVisible: false,
        gameInstructionVisible: true,
        sequenceDisplayVisible: false,
        cardGridVisible: false,
        submitButtonVisible: false,
        resetButtonVisible: false,
        userLostGridVisible: false
    });

    DOM.scoreDisplay.textContent = GameState.score;
};

// Start Game
const startGame = async () => {
    try {
        updateUI({
            startBtnVisible: false,
            gameInstructionVisible: false,
            scoreVisible: true
        });

        const userId = generateUserId();
        const response = await fetch('https://localhost:7214/api/Game/start', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userId)
        });

        if (!response.ok) throw new Error('Failed to start the game');

        const gameState = await response.json();
        Object.assign(GameState, {
            gameId: gameState.gameId,
            correctSequence: gameState.correctSequence,
            score: 0,
            currentLevel: gameState.currentLevel
        });

        DOM.scoreDisplay.textContent = GameState.score;
        showSequence();
    } catch (error) {
        alert(error.message);
    }
};

// Generate Unique User ID
const generateUserId = () => "user_" + Math.random().toString(36).substring(7);

// Update UI States
const updateUI = ({
    startBtnVisible = false,
    scoreVisible = false,
    correctCardGridVisible = false,
    incorrectCardGridVisible = false,
    gameInstructionVisible = false,
    sequenceDisplayVisible = false,
    sequenceCardsVisible = false,
    cardGridVisible = false,
    submitButtonVisible = false,
    resetButtonVisible = false,
    userLostGridVisible = false
} = {}) => {
    DOM.correctCardGrid.style.display = correctCardGridVisible ? "flex" : "none";
    DOM.incorrectCardGrid.style.display = incorrectCardGridVisible ? "flex" : "none";
    DOM.startBtn.style.display = startBtnVisible ? "inline-block" : "none";
    DOM.scoreDiv.style.display = scoreVisible ? "block" : "none";
    DOM.gameInstruction.style.display = gameInstructionVisible ? "flex" : "none";
    DOM.sequenceDisplay.style.display = sequenceDisplayVisible ? "flex" : "none";
    DOM.sequenceCards.style.display = sequenceCardsVisible ? "flex" : "none";
    DOM.cardGrid.style.display = cardGridVisible ? "grid" : "none";
    DOM.submitButton.style.display = submitButtonVisible ? "block" : "none";
    DOM.resetButton.style.display = resetButtonVisible ? "block" : "none";
    DOM.userLostGrid.style.display = userLostGridVisible ? "flex" : "none";
};

// Show Sequence to Remember
const showSequence = () => {
    updateUI({ sequenceDisplayVisible: true , sequenceCardsVisible: true , scoreVisible :  true});
    renderCards(DOM.sequenceCards, GameState.correctSequence);

    setTimeout(() => {
        updateUI({ sequenceDisplayVisible: false });
        showCardGrid();
    }, 2000);
};

// Render Cards
const renderCards = (container, cards) => {
    container.innerHTML = '';
    cards.forEach(card => {
        const cardElement = createCardElement(card);
        container.appendChild(cardElement);
    });

    container.style.gridTemplateColumns = `repeat(${Math.min(cards.length, 6)}, 1fr)`;
};

// Create Card Element
const createCardElement = (card) => {
    const cardElement = document.createElement('div');
    cardElement.classList.add('card');
    cardElement.style.backgroundImage = `url(images/${card}.png)`;
    cardElement.dataset.card = card;
    return cardElement;
};

// Show Card Grid for User Selection
const showCardGrid = () => {
    const gridCards = generateGridCards(GameState.correctSequence, 24);
    DOM.cardGrid.innerHTML = '';
    
    gridCards.forEach(card => {
        const cardElement = createCardElement(card);
        cardElement.addEventListener('click', () => selectCard(cardElement));
        DOM.cardGrid.appendChild(cardElement);
    });

    updateUI({ cardGridVisible: true, submitButtonVisible: true , scoreVisible : true});
};

// Generate Grid Cards with Random Cards and Correct Sequence
const generateGridCards = (sequence, totalCards) => {
    const gridCards = [...sequence];

    while (gridCards.length < totalCards) {
        const randomCard = generateSequence(1)[0];
        if (!gridCards.includes(randomCard)) gridCards.push(randomCard);
    }

    return shuffleArray(gridCards);
};

// Shuffle Array
const shuffleArray = (array) => {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
};

// Select Card
const selectCard = (cardElement) => {
    if (GameState.userSequence.length < GameState.currentLevel) {
        cardElement.classList.toggle('selected');
        const card = cardElement.dataset.card;

        if (GameState.userSequence.includes(card)) {
            GameState.userSequence = GameState.userSequence.filter(item => item !== card);
        } else {
            GameState.userSequence.push(card);
        }
    }
};

// Submit User Sequence
const submitSequence = async () => {
    if (GameState.userSequence.length !== GameState.currentLevel) {
        alert('Please select the correct number of cards');
        return;
    }

    try {
        const response = await fetch(`https://localhost:7214/api/Game/${GameState.gameId}/submit`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(GameState.userSequence)
        });

        if (!response.ok) throw new Error(await response.text());

        const gameState = await response.json();
        Object.assign(GameState, {
            currentLevel: gameState.currentLevel,
            correctSequence: gameState.correctSequence,
            userSequence: []
        });

        DOM.scoreDisplay.textContent = gameState.currentLevel - 1;

        if (GameState.currentLevel === 10) {
            alert('You win!');
        } else {
            showSequence();
        }
    } catch (error) {
        handleGameOver(error.message);
    }
};

const handleGameOver = (error) => {
    const [incorrectCards, correctCards] = error.split(';').map(seq => seq.split(','));

    renderCards(DOM.incorrectCardGrid, incorrectCards);
    renderCards(DOM.correctCardGrid, correctCards);

    updateUI({
        userLostGridVisible: true,
        resetButtonVisible: true,
        incorrectCardGridVisible : true,
        correctCardGridVisible : true,
        scoreVisible: false,
        submitButtonVisible: false
    });
};

// Generate Sequence
const generateSequence = (level) => {
    const suits = ["clubs", "diamonds", "hearts", "spades"];
    const ranks = ["A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K"];

    return Array.from({ length: level }, () => {
        const suit = suits[Math.floor(Math.random() * suits.length)];
        const rank = ranks[Math.floor(Math.random() * ranks.length)];
        return `${suit}_${rank}`;
    });
};

// Initialize
initGame();
