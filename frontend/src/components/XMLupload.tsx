import React, { useState, ChangeEvent, FormEvent } from "react";

function XmlUpload() {
    const [xmlFile, setXmlFile] = useState<File | null>(null);
    const [message, setMessage] = useState<string>("");

    const handleFileChange = (e: ChangeEvent<HTMLInputElement>) => {
        if (e.target.files) {
            setXmlFile(e.target.files[0]);
            setMessage(""); // Clear message when selecting a new file
        }
    };

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (!xmlFile) {
            setMessage("Please select an XML file.");
            return;
        }

        const formData = new FormData();
        formData.append("xmlFile", xmlFile);

        try {
            const response = await fetch("http://localhost:5000/api/XmlImport/upload", {
                method: "POST",
                body: formData,
            });

            if (response.ok) {
                const result = await response.json();
                setMessage(result.message || "File imported successfully!");
            } else {
                const errorText = await response.text();
                setMessage(`Upload failed: ${errorText}`);
            }
        } catch (error: any) {
            setMessage(`Error: ${error.message}`);
        }
    };

    return (
        <div
            style={{
                maxWidth: '600px',
                margin: '0 auto',
                padding: '2rem',
            }}
        >
            <h3
                style={{
                    marginBottom: '1.5rem',
                    fontSize: '0.95rem',
                    fontWeight: '500',
                }}
            >
                Kies hier het XML-bestand dat u wilt uploaden naar de huidige database.
            </h3>
            <form
                onSubmit={handleSubmit}
                style={{
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '1.5rem',
                }}
            >
                <div>
                    <label
                        style={{
                            display: 'block',
                            fontWeight: '600',
                            marginBottom: '0.5rem',
                        }}
                    >
                        XML bestand:
                    </label>
                    <input type="file" accept=".xml" onChange={handleFileChange} />
                    {xmlFile && (
                        <p style={{ marginTop: '0.5rem', fontStyle: 'italic' }}>
                            Geselecteerd bestand: <strong>{xmlFile.name}</strong>
                        </p>
                    )}
                </div>

                <button type="submit">Upload</button>
            </form>

            {message && <p style={{ marginTop: '1rem', fontWeight: '500' }}>{message}</p>}
        </div>
    );
}

export default XmlUpload;