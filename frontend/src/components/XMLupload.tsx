
import React, { useState, ChangeEvent, FormEvent } from "react";

function XmlUpload() {
    const [xmlFile1, setXmlFile1] = useState<File | null>(null);
    const [xmlFile2, setXmlFile2] = useState<File | null>(null);
    const [message, setMessage] = useState<string>("");

    const handleFile1Change = (e: ChangeEvent<HTMLInputElement>) => {
        if (e.target.files) setXmlFile1(e.target.files[0]);
    };

    const handleFile2Change = (e: ChangeEvent<HTMLInputElement>) => {
        if (e.target.files) setXmlFile2(e.target.files[0]);
    };

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if (!xmlFile1 || !xmlFile2) {
            setMessage("Please select both XML files.");
            return;
        }

        const formData = new FormData();
        formData.append("xmlFile1", xmlFile1);
        formData.append("xmlFile2", xmlFile2);

        try {
            const response = await fetch("http://localhost:5000/api/XmlImport/upload", {
                method: "POST",
                body: formData,
            });

            if (response.ok) {
                const result = await response.json();
                setMessage(result.message || "Files imported successfully!");
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
                Kies hier de bestanden die u wilt uploaden, deze bestanden zullen worden geupload naar de huidige database.
            </h3>
            <form
                onSubmit={handleSubmit}
                style={{
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '1.5rem',
                }}
            >
                {/* Row for both file inputs */}
                <div
                    style={{
                        display: 'flex',
                        gap: '1rem',
                        justifyContent: 'space-between',
                    }}
                >
                    <div style={{ flex: 1 }}>
                        <label
                            style={{
                                display: 'block',
                                fontWeight: '600',
                                marginBottom: '0.5rem',
                            }}
                        >
                            XML bestand 1:
                        </label>
                        <label className="custom-file-upload">
                            <input type="file" accept=".xml" onChange={handleFile1Change} />
                        </label>
                    </div>

                    <div style={{ flex: 1 }}>
                        <label
                            style={{
                                display: 'block',
                                fontWeight: '600',
                                marginBottom: '0.5rem',
                            }}
                        >
                            XML bestand 2:
                        </label>
                        <label className="custom-file-upload">
                            <input type="file" accept=".xml" onChange={handleFile2Change} />
                        </label>
                    </div>
                </div>

                <button type="submit">Upload</button>
            </form>

            {message && <p style={{ marginTop: '1rem', fontWeight: '500' }}>{message}</p>}
        </div>
    );
}

export default XmlUpload;