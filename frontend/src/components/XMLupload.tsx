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
            const response = await fetch("/api/XmlImport/upload", {
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
            <h2>Upload XML Files</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>
                        XML File 1:
                        <input type="file" accept=".xml" onChange={handleFile1Change} />
                    </label>
                </div>
                <div>
                    <label>
                        XML File 2:
                        <input type="file" accept=".xml" onChange={handleFile2Change} />
                    </label>
                </div>
                <button type="submit">Upload and Import</button>
            </form>
            {message && <p>{message}</p>}
        </div>
    );
}

export default XmlUpload;