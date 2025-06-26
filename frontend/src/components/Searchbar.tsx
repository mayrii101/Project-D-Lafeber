import React from 'react';
import '../styles/Searchbar.css';

interface SearchBarProps {
    searchTerm: string;
    onSearchChange: (term: string) => void;
    placeholder?: string; // 🔹 optioneel veld toevoegen
}

const SearchBar: React.FC<SearchBarProps> = ({ searchTerm, onSearchChange, placeholder }) => {
    return (
        <div className="search-container">
            <input
                type="text"
                placeholder={placeholder || "Zoek..."} // 🔹 dynamisch of standaard tekst
                value={searchTerm}
                onChange={(e) => onSearchChange(e.target.value)}
            />
        </div>
    );
};

export default SearchBar;
