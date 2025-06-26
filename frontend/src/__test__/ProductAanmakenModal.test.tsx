import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import ProductAanmakenModal from "../components/ProductAanmakenModal";
import "@testing-library/jest-dom";

global.fetch = jest.fn(() =>
  Promise.resolve({ ok: true, json: () => Promise.resolve({}) })
) as jest.Mock;

describe("ProductAanmakenModal", () => {
  const mockOnSuccess = jest.fn();
  const mockOnClose = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
    render(<ProductAanmakenModal onClose={mockOnClose} onSuccess={mockOnSuccess} />);
  });

  it("shows validation errors for empty fields", async () => {
    fireEvent.click(screen.getByRole("button", { name: /aanmaken/i }));

    expect(await screen.findByText(/productnaam mag niet leeg zijn/i)).toBeInTheDocument();
    expect(screen.getByText(/gewicht moet een positief geheel getal zijn/i)).toBeInTheDocument();
    expect(screen.getByText(/batchnummer moet een positief geheel getal zijn/i)).toBeInTheDocument();
    expect(screen.getByText(/prijs moet 0 of hoger zijn/i)).toBeInTheDocument();
    expect(screen.getByText(/categorie mag niet leeg zijn/i)).toBeInTheDocument();
  });

  it("accepts valid input and submits the form", async () => {
    fireEvent.change(screen.getByLabelText(/productnaam/i), { target: { value: "Laptop" } });
    fireEvent.change(screen.getByLabelText(/sku/i), { target: { value: "SKU123" } });
    fireEvent.change(screen.getByLabelText(/gewicht/i), { target: { value: "3" } });
    fireEvent.change(screen.getByLabelText(/materiaal/i), { target: { value: "Aluminium" } });
    fireEvent.change(screen.getByLabelText(/batchnummer/i), { target: { value: "101" } });
    fireEvent.change(screen.getByLabelText(/prijs/i), { target: { value: "999.99" } });
    fireEvent.change(screen.getByLabelText(/categorie/i), { target: { value: "Elektronica" } });

    fireEvent.click(screen.getByRole("button", { name: /aanmaken/i }));

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith(
        "http://localhost:5000/api/product",
        expect.objectContaining({
          method: "POST",
          headers: { "Content-Type": "application/json" },
        })
      );
      expect(mockOnSuccess).toHaveBeenCalled();
      expect(mockOnClose).toHaveBeenCalled();
    });
  });

  it("does not call onSuccess or onClose if backend response is not ok", async () => {
    (global.fetch as jest.Mock).mockResolvedValueOnce({ ok: false });

    fireEvent.change(screen.getByLabelText(/productnaam/i), { target: { value: "Test" } });
    fireEvent.change(screen.getByLabelText(/gewicht/i), { target: { value: "1" } });
    fireEvent.change(screen.getByLabelText(/batchnummer/i), { target: { value: "1" } });
    fireEvent.change(screen.getByLabelText(/prijs/i), { target: { value: "1" } });
    fireEvent.change(screen.getByLabelText(/categorie/i), { target: { value: "TestCat" } });

    fireEvent.click(screen.getByRole("button", { name: /aanmaken/i }));

    await waitFor(() => {
      expect(mockOnSuccess).not.toHaveBeenCalled();
      expect(mockOnClose).not.toHaveBeenCalled();
    });
  });
});
