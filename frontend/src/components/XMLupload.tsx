
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
        <div>
            <h3 style={{ marginBottom: '1.5rem' }}>Kies hier de bestanden die u wilt uploaden</h3>
            <form onSubmit={handleSubmit} style={{ display: 'flex', gap: '2rem', alignItems: 'center', marginBottom: '1rem' }}>
                <div style={{ flex: 1 }}>
                    <label style={{ display: 'block', marginBottom: '0.5rem', fontWeight: '600' }}>
                        XML File 1:
                    </label>
                    <input type="file" accept=".xml" onChange={handleFile1Change} />
                </div>
                <div style={{ flex: 1 }}>
                    <label style={{ display: 'block', marginBottom: '0.5rem', fontWeight: '600' }}>
                        XML File 2:
                    </label>
                    <input type="file" accept=".xml" onChange={handleFile2Change} />
                </div>
                <button type="submit" style={{ height: '2.8rem', alignSelf: 'flex-end', padding: '0 1.2rem', fontWeight: '600' }}>
                    Upload and Import
                </button>
            </form>
            {message && <p style={{ marginTop: '1rem', fontWeight: '500' }}>{message}</p>}
        </div>
    );
}

export default XmlUpload;