import React from "react";
import "../styles/Filter.css"; // We'll create this new CSS file

interface FilterProps {
    selectedStatus: string;
    onStatusChange: (status: string) => void;
}

const Filter: React.FC<FilterProps> = ({ selectedStatus, onStatusChange }) => {
    return (
        <div className="filter-wrapper">
            <select
                className="status-filter"
                value={selectedStatus}
                onChange={(e) => onStatusChange(e.target.value)}
            >
                <option value="">Alle status</option>
                <option value="pending">In afwachting</option>
                <option value="processing">wordt verwerkt</option>
                <option value="shipped">Verzonden</option>
                <option value="delivered">Geleverd</option>
                <option value="cancelled">Geannuleerd</option>
            </select>
        </div>
    );
};

export default Filter;