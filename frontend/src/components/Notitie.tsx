import React, { useEffect, useState } from "react";

interface Props {
    visible: boolean;
    onClose: () => void;
}

const Notitie: React.FC<Props> = ({ visible, onClose }) => {
    const [notes, setNotes] = useState("");

    useEffect(() => {
        fetch("http://localhost:5000/api/stickynote")
            .then((res) => res.json())
            .then((data) => {
                setNotes(data.content || "");
            });
    }, []);

    useEffect(() => {
        const timeout = setTimeout(() => {
            fetch("http://localhost:5000/api/stickynote", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(notes),
            });
        }, 800);

        return () => clearTimeout(timeout);
    }, [notes]);

    if (!visible) return null;

    return (
        <div className={`sticky-note-panel open`}>
            <textarea
                className="sticky-note-textarea"
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
                placeholder="Typ hier je notities..."
            />
            <div className="sticky-note-toggle open" onClick={onClose}>
                📝
            </div>
        </div>
    );
};

export default Notitie;
