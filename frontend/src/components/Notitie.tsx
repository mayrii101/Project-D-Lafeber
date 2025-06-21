import React, { useEffect, useState } from "react";

const Notitie: React.FC = () => {
    const [visible, setVisible] = useState(false);
    const [notes, setNotes] = useState("");

    const toggleVisibility = () => setVisible((prev) => !prev);

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

    return (
        <>
            <div
                className={`sticky-note-toggle ${visible ? "open" : ""}`}
                onClick={toggleVisibility}
            >
                📝
            </div>

            <div className={`sticky-note-panel ${visible ? "open" : ""}`}>
                <textarea
                    className="sticky-note-textarea"
                    value={notes}
                    onChange={(e) => setNotes(e.target.value)}
                    placeholder="Typ hier je notities..."
                />
            </div>
        </>
    );
};

export default Notitie;
